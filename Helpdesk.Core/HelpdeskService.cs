using System;
using Helpdesk.Core.Models;
using Helpdesk.Core.Interfaces;

namespace Helpdesk.Core
{
    public class HelpdeskService
    {
        ITicketRepository tickets;
        IBaseConnaissanceRepository kb;
        Random r = new Random();

        public HelpdeskService(ITicketRepository t, IBaseConnaissanceRepository k)
        {
            tickets = t;
            kb = k;
        }

        public int CreerTicket(int clientId, int catId, string titre, string desc)
        {
            Ticket t = new Ticket();
            t.ClientId = clientId;
            t.CategorieId = catId;
            t.Titre = titre;
            t.Description = desc;
            t.NiveauId = 1;
            t.Statut = "Ouvert";

            int id = tickets.Create(t);
            tickets.AddHistorique(id, "ticket ouvert");
            return id;
        }

        public void TraiterTicket(int id)
        {
            Ticket? t = tickets.GetById(id);
            if (t == null) return;
            if (t.Statut == "Resolu" || t.Statut == "Non resolu") return;

            Console.WriteLine("niveau " + t.NiveauId + " regarde le ticket...");

            var sols = kb.GetByCategorie(t.CategorieId);
            bool ok = false;

            if (sols.Count > 0)
            {
                // cas demo : mdp n2, appli n2, vpn n3, serveur jamais
                if (t.CategorieId == 1 && t.NiveauId == 2)
                    ok = true;
                else if (t.CategorieId == 5 && t.NiveauId == 2)
                    ok = true;
                else if (t.CategorieId == 7 && t.NiveauId == 3)
                    ok = true;
                else if (t.CategorieId == 3)
                    ok = r.Next(100) < (40 + t.NiveauId * 20);
                else if (t.CategorieId != 5 && t.CategorieId != 7 && t.CategorieId != 6)
                    ok = r.Next(100) < (25 + t.NiveauId * 20);
            }

            if (ok)
            {
                t.Statut = "Resolu";
                t.DateResolution = DateTime.Now;
                tickets.Update(t);
                tickets.AddHistorique(id, "resolu n" + t.NiveauId);
                Console.WriteLine("resolu : " + sols[0].Solution);
            }
            else if (t.NiveauId < 3)
            {
                int old = t.NiveauId;
                t.NiveauId++;
                t.Statut = "En cours";
                tickets.Update(t);
                tickets.AddHistorique(id, "passe au n" + t.NiveauId);
                Console.WriteLine("n" + old + " galere, on monte au n" + t.NiveauId);
            }
            else if (t.CategorieId == 6)
            {
                t.Statut = "Non resolu";
                tickets.Update(t);
                tickets.AddHistorique(id, "bloque");
                Console.WriteLine("incident non resolu");
            }
            else if (t.CategorieId == 3 && r.Next(100) < 35)
            {
                t.Statut = "Non resolu";
                tickets.Update(t);
                tickets.AddHistorique(id, "imprimante hs");
                Console.WriteLine("incident non resolu");
            }
            else
            {
                string solution = sols.Count > 0 ? sols[0].Solution : "redemarrer le pc ou le service";

                t.Statut = "Resolu";
                t.DateResolution = DateTime.Now;
                tickets.Update(t);
                tickets.AddHistorique(id, "resolu n3");

                BaseConnaissance a = new BaseConnaissance();
                a.CategorieId = t.CategorieId;
                a.Sujet = "fix " + t.Titre;
                a.Solution = solution;
                kb.Add(a);

                Console.WriteLine("resolu : " + solution);
            }
        }

        public void AfficherStats()
        {
            var all = tickets.GetAll();
            int resolus = 0;
            int nonResolus = 0;
            int enCours = 0;

            Console.WriteLine();
            Console.WriteLine("recap fin demo :");
            foreach (var t in all)
            {
                string info = "ticket " + t.TicketId;
                if (t.Titre.Length > 0)
                    info += " (" + t.Titre.Split('-')[0].Trim() + ")";
                info += " -> " + t.Statut.ToLower();
                if (t.Statut == "Resolu")
                    info += " n" + t.NiveauId;
                Console.WriteLine(info);

                if (t.Statut == "Resolu") resolus++;
                else if (t.Statut == "Non resolu") nonResolus++;
                else enCours++;
            }

            Console.WriteLine("total " + all.Count + " | " + resolus + " resolus | " + nonResolus + " bloques"
                + (enCours > 0 ? " | " + enCours + " en cours" : ""));
        }
    }
}
