using System;

namespace Helpdesk.Core.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Nom { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class CategorieIncident
    {
        public int CategorieId { get; set; }
        public string NomCategorie { get; set; } = "";
    }

    public class Ticket
    {
        public int TicketId { get; set; }
        public int ClientId { get; set; }
        public int CategorieId { get; set; }
        public int NiveauId { get; set; }
        public string Titre { get; set; } = "";
        public string Description { get; set; } = "";
        public string Statut { get; set; } = "Ouvert";
        public DateTime DateCreation { get; set; } = DateTime.Now;
        public DateTime? DateResolution { get; set; }
    }

    public class BaseConnaissance
    {
        public int ArticleId { get; set; }
        public int CategorieId { get; set; }
        public string Sujet { get; set; } = "";
        public string Solution { get; set; } = "";
    }

    public class Materiel
    {
        public int MaterielId { get; set; }
        public int ClientId { get; set; }
        public string Nom { get; set; } = "";
        public string Type { get; set; } = "";
    }

}
