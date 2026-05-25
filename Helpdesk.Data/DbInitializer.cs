using System;
using Npgsql;

namespace Helpdesk.Data
{
    public class DbInitializer
    {
        string cs;

        public DbInitializer(string connectionString)
        {
            cs = connectionString;
        }

        public void Initialize()
        {
            using var conn = new NpgsqlConnection(cs);
            conn.Open();

            ViderTables(conn);
            CreerTables(conn);
            InsererDonnees(conn);
        }

        void Executer(NpgsqlConnection conn, string sql)
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        void ViderTables(NpgsqlConnection conn)
        {
            try
            {
                Executer(conn, @"
                    TRUNCATE TABLE HistoriqueTickets, Interventions, Appels, Tickets, Materiel,
                    Clients, CategoriesIncidents, NiveauxSupport, Techniciens RESTART IDENTITY CASCADE;
                ");
            }
            catch { }
        }

        void CreerTables(NpgsqlConnection conn)
        {
            Executer(conn, @"
                CREATE TABLE IF NOT EXISTS Clients (
                    ClientId SERIAL PRIMARY KEY,
                    Nom VARCHAR(100) NOT NULL,
                    Email VARCHAR(100) UNIQUE NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Techniciens (
                    TechnicienId SERIAL PRIMARY KEY,
                    Nom VARCHAR(100) NOT NULL,
                    Specialite VARCHAR(100)
                );
                CREATE TABLE IF NOT EXISTS NiveauxSupport (
                    NiveauId SERIAL PRIMARY KEY,
                    NomNiveau VARCHAR(50) UNIQUE NOT NULL
                );
                CREATE TABLE IF NOT EXISTS CategoriesIncidents (
                    CategorieId SERIAL PRIMARY KEY,
                    NomCategorie VARCHAR(100) UNIQUE NOT NULL
                );
            ");

            Executer(conn, @"
                CREATE TABLE IF NOT EXISTS Tickets (
                    TicketId SERIAL PRIMARY KEY,
                    ClientId INT NOT NULL REFERENCES Clients(ClientId),
                    CategorieId INT NOT NULL REFERENCES CategoriesIncidents(CategorieId),
                    NiveauId INT NOT NULL REFERENCES NiveauxSupport(NiveauId),
                    Titre VARCHAR(255) NOT NULL,
                    Description TEXT,
                    Statut VARCHAR(20) NOT NULL DEFAULT 'Ouvert',
                    DateCreation TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    DateResolution TIMESTAMP
                );
                CREATE TABLE IF NOT EXISTS Interventions (
                    InterventionId SERIAL PRIMARY KEY,
                    TicketId INT NOT NULL REFERENCES Tickets(TicketId),
                    TechnicienId INT NOT NULL REFERENCES Techniciens(TechnicienId),
                    DescriptionAction TEXT NOT NULL,
                    DateIntervention TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
                CREATE TABLE IF NOT EXISTS BaseConnaissances (
                    ArticleId SERIAL PRIMARY KEY,
                    CategorieId INT NOT NULL REFERENCES CategoriesIncidents(CategorieId),
                    Sujet VARCHAR(255) NOT NULL,
                    Solution TEXT NOT NULL,
                    DateAjout TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
            ");

            Executer(conn, @"
                CREATE TABLE IF NOT EXISTS HistoriqueTickets (
                    HistoriqueId SERIAL PRIMARY KEY,
                    TicketId INT NOT NULL REFERENCES Tickets(TicketId),
                    Action VARCHAR(255) NOT NULL,
                    DateAction TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
                CREATE TABLE IF NOT EXISTS Materiel (
                    MaterielId SERIAL PRIMARY KEY,
                    ClientId INT NOT NULL REFERENCES Clients(ClientId),
                    Nom VARCHAR(100) NOT NULL,
                    Type VARCHAR(50) NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Appels (
                    AppelId SERIAL PRIMARY KEY,
                    ClientId INT NOT NULL REFERENCES Clients(ClientId),
                    TicketId INT NOT NULL REFERENCES Tickets(TicketId),
                    Notes TEXT,
                    DateAppel TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
            ");
        }

        void InsererDonnees(NpgsqlConnection conn)
        {
            InsererClientsEtTech(conn);
            InsererCategories(conn);
            InsererKb(conn);
            InsererMateriel(conn);
        }

        void InsererClientsEtTech(NpgsqlConnection conn)
        {
            Executer(conn, @"
                INSERT INTO Clients (Nom, Email) VALUES
                ('Geoffroy', 'geoffroy@example.com'),
                ('Michael', 'michael@example.com'),
                ('Hamza', 'hamza@example.com'),
                ('Sarah', 'sarah@example.com'),
                ('Yassine', 'yassine@example.com');
                INSERT INTO Techniciens (Nom, Specialite) VALUES
                ('Alice', 'N1'), ('Bob', 'N2'), ('Charlie', 'N3');
                INSERT INTO NiveauxSupport (NomNiveau) VALUES
                ('Niveau 1'), ('Niveau 2'), ('Niveau 3');
            ");
        }

        void InsererCategories(NpgsqlConnection conn)
        {
            Executer(conn, @"
                INSERT INTO CategoriesIncidents (NomCategorie) VALUES
                ('Mot de passe oublié'), ('Ordinateur lent'),
                ('Imprimante non détectée'), ('Connexion réseau impossible'),
                ('Erreur logiciel'), ('Problème de serveur'),
                ('VPN inaccessible'), ('Email professionnel bloqué');
            ");
        }

        void InsererKb(NpgsqlConnection conn)
        {
            Executer(conn, @"
                DELETE FROM BaseConnaissances;
                INSERT INTO BaseConnaissances (CategorieId, Sujet, Solution) VALUES
                (1, 'Reset mdp', 'Reinitialiser le mot de passe sur le portail IT.'),
                (3, 'Imprimante HS', 'Verifier le cable, redemarrer l imprimante et le spooler.'),
                (4, 'Pas de reseau', 'Verifier le wifi et rebooter la box internet.'),
                (5, 'Appli crash', 'Desactiver l addon, vider le cache puis relancer l appli.'),
                (6, 'Serveur down', 'Verifier les logs et redemarrer les services du serveur.'),
                (7, 'VPN coupe', 'Reinstaller le certificat VPN et reconfigurer le tunnel.');
            ");
        }

        void InsererMateriel(NpgsqlConnection conn)
        {
            Executer(conn, @"
                INSERT INTO Materiel (ClientId, Nom, Type) VALUES
                (1, 'PC-Bureau-01', 'PC'), (2, 'PC-Portable-02', 'PC'),
                (3, 'PC-Maison-03', 'PC'), (4, 'Imprimante-HP-04', 'Imprimante'),
                (5, 'Serveur-Etage3', 'Serveur');
            ");
        }
    }
}
