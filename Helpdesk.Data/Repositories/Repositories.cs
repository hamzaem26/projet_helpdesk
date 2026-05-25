using System;
using System.Collections.Generic;
using Npgsql;
using Helpdesk.Core.Models;
using Helpdesk.Core.Interfaces;

namespace Helpdesk.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        string cs;

        public TicketRepository(string connectionString)
        {
            cs = connectionString;
        }

        public int Create(Ticket ticket)
        {
            string sql = "INSERT INTO Tickets (ClientId, CategorieId, NiveauId, Titre, Description, Statut) " +
                         "VALUES (@c, @cat, @n, @t, @d, @s) RETURNING TicketId";

            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("c", ticket.ClientId);
            cmd.Parameters.AddWithValue("cat", ticket.CategorieId);
            cmd.Parameters.AddWithValue("n", ticket.NiveauId);
            cmd.Parameters.AddWithValue("t", ticket.Titre);
            cmd.Parameters.AddWithValue("d", ticket.Description);
            cmd.Parameters.AddWithValue("s", ticket.Statut);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public Ticket? GetById(int id)
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM Tickets WHERE TicketId = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            Ticket t = new Ticket();
            t.TicketId = (int)r["TicketId"];
            t.ClientId = (int)r["ClientId"];
            t.CategorieId = (int)r["CategorieId"];
            t.NiveauId = (int)r["NiveauId"];
            t.Titre = r["Titre"]?.ToString() ?? "";
            t.Description = r["Description"]?.ToString() ?? "";
            t.Statut = r["Statut"]?.ToString() ?? "Ouvert";
            t.DateCreation = (DateTime)r["DateCreation"];
            t.DateResolution = r["DateResolution"] as DateTime?;
            return t;
        }

        public void Update(Ticket ticket)
        {
            string sql = "UPDATE Tickets SET Statut=@s, NiveauId=@n, DateResolution=@dr WHERE TicketId=@id";
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("s", ticket.Statut);
            cmd.Parameters.AddWithValue("n", ticket.NiveauId);
            cmd.Parameters.AddWithValue("dr", (object?)ticket.DateResolution ?? DBNull.Value);
            cmd.Parameters.AddWithValue("id", ticket.TicketId);
            cmd.ExecuteNonQuery();
        }

        public List<Ticket> GetAll()
        {
            List<Ticket> liste = new List<Ticket>();
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT TicketId, Titre, Statut, NiveauId FROM Tickets", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Ticket t = new Ticket();
                t.TicketId = (int)r["TicketId"];
                t.Titre = r["Titre"]?.ToString() ?? "";
                t.Statut = r["Statut"]?.ToString() ?? "Ouvert";
                t.NiveauId = (int)r["NiveauId"];
                liste.Add(t);
            }
            return liste;
        }

        public void AddHistorique(int ticketId, string action)
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("INSERT INTO HistoriqueTickets (TicketId, Action) VALUES (@id, @a)", conn);
            cmd.Parameters.AddWithValue("id", ticketId);
            cmd.Parameters.AddWithValue("a", action);
            cmd.ExecuteNonQuery();
        }
    }

    public class BaseConnaissanceRepository : IBaseConnaissanceRepository
    {
        string cs;

        public BaseConnaissanceRepository(string connectionString)
        {
            cs = connectionString;
        }

        public void Add(BaseConnaissance article)
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("INSERT INTO BaseConnaissances (CategorieId, Sujet, Solution) VALUES (@c, @s, @sol)", conn);
            cmd.Parameters.AddWithValue("c", article.CategorieId);
            cmd.Parameters.AddWithValue("s", article.Sujet);
            cmd.Parameters.AddWithValue("sol", article.Solution);
            cmd.ExecuteNonQuery();
        }

        public List<BaseConnaissance> GetByCategorie(int categorieId)
        {
            List<BaseConnaissance> liste = new List<BaseConnaissance>();
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM BaseConnaissances WHERE CategorieId = @id", conn);
            cmd.Parameters.AddWithValue("id", categorieId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                BaseConnaissance a = new BaseConnaissance();
                a.Sujet = r["Sujet"]?.ToString() ?? "";
                a.Solution = r["Solution"]?.ToString() ?? "";
                liste.Add(a);
            }
            return liste;
        }

        public List<BaseConnaissance> Search(string query)
        {
            List<BaseConnaissance> liste = new List<BaseConnaissance>();
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM BaseConnaissances WHERE Sujet ILIKE @q OR Solution ILIKE @q", conn);
            cmd.Parameters.AddWithValue("q", "%" + query + "%");
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                BaseConnaissance a = new BaseConnaissance();
                a.Sujet = r["Sujet"]?.ToString() ?? "";
                a.Solution = r["Solution"]?.ToString() ?? "";
                liste.Add(a);
            }
            return liste;
        }
    }

    public class DonneesRepository
    {
        string cs;

        public DonneesRepository(string connectionString)
        {
            cs = connectionString;
        }

        public List<Client> GetClients()
        {
            List<Client> liste = new List<Client>();
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM Clients ORDER BY ClientId", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Client c = new Client();
                c.ClientId = (int)r["ClientId"];
                c.Nom = r["Nom"]?.ToString() ?? "";
                liste.Add(c);
            }
            return liste;
        }

        public List<CategorieIncident> GetCategories()
        {
            List<CategorieIncident> liste = new List<CategorieIncident>();
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM CategoriesIncidents ORDER BY CategorieId", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                CategorieIncident cat = new CategorieIncident();
                cat.CategorieId = (int)r["CategorieId"];
                cat.NomCategorie = r["NomCategorie"]?.ToString() ?? "";
                liste.Add(cat);
            }
            return liste;
        }

        public Materiel? GetMateriel(int clientId)
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM Materiel WHERE ClientId = @id LIMIT 1", conn);
            cmd.Parameters.AddWithValue("id", clientId);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            Materiel m = new Materiel();
            m.Nom = r["Nom"]?.ToString() ?? "";
            m.Type = r["Type"]?.ToString() ?? "";
            return m;
        }

        public void AddAppel(int clientId, int ticketId, string notes)
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("INSERT INTO Appels (ClientId, TicketId, Notes) VALUES (@c, @t, @n)", conn);
            cmd.Parameters.AddWithValue("c", clientId);
            cmd.Parameters.AddWithValue("t", ticketId);
            cmd.Parameters.AddWithValue("n", notes);
            cmd.ExecuteNonQuery();
        }

        public void AddIntervention(int ticketId, int techId, string action)
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("INSERT INTO Interventions (TicketId, TechnicienId, DescriptionAction) VALUES (@t, @tech, @a)", conn);
            cmd.Parameters.AddWithValue("t", ticketId);
            cmd.Parameters.AddWithValue("tech", techId);
            cmd.Parameters.AddWithValue("a", action);
            cmd.ExecuteNonQuery();
        }

        public string GetTech(int techId)
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT Nom FROM Techniciens WHERE TechnicienId = @id", conn);
            cmd.Parameters.AddWithValue("id", techId);
            object? res = cmd.ExecuteScalar();
            if (res == null) return "Technicien";
            return res.ToString() ?? "Technicien";
        }
    }
}
