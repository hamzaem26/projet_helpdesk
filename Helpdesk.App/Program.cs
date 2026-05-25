using System;
using System.Threading;
using Helpdesk.Data;
using Helpdesk.Data.Repositories;
using Helpdesk.Core;
using Helpdesk.Core.Models;

namespace Helpdesk.App
{
    class Program
    {
        class Scenario
        {
            public int IdxClient;
            public int IdxCat;
            public string Titre = "";
            public string Probleme = "";
        }

        static string connexion = "Host=localhost;Port=5432;Database=helpdesk_db;Username=postgres;Password=commey123";

        static void Main(string[] args)
        {
            Console.WriteLine("helpdesk system");
            Console.WriteLine();

            try
            {
                DbInitializer init = new DbInitializer(connexion);
                init.Initialize();
                Console.WriteLine("base ok");
                Console.WriteLine();

                TicketRepository tickets = new TicketRepository(connexion);
                BaseConnaissanceRepository kb = new BaseConnaissanceRepository(connexion);
                DonneesRepository data = new DonneesRepository(connexion);
                HelpdeskService service = new HelpdeskService(tickets, kb);

                var clients = data.GetClients();
                var categories = data.GetCategories();

                Console.WriteLine("demo : geoffroy michael hamza sarah yassine");
                Console.WriteLine();

                Scenario[] cas = new Scenario[5];

                cas[0] = new Scenario
                {
                    IdxClient = 0, IdxCat = 0,
                    Titre = "Geoffroy - Mot de passe oublie",
                    Probleme = "Geoffroy a oublie son mot de passe."
                };

                cas[1] = new Scenario
                {
                    IdxClient = 1, IdxCat = 4,
                    Titre = "Michael - Appli qui crash",
                    Probleme = "Michael : appli se ferme a l enregistrement."
                };

                cas[2] = new Scenario
                {
                    IdxClient = 2, IdxCat = 6,
                    Titre = "Hamza - VPN inaccessible",
                    Probleme = "Hamza : VPN coupe, certificat expire, plus acces au reseau entreprise."
                };

                cas[3] = new Scenario
                {
                    IdxClient = 3, IdxCat = 2,
                    Titre = "Sarah - Imprimante en panne",
                    Probleme = "Sarah : imprimante marche plus."
                };

                cas[4] = new Scenario
                {
                    IdxClient = 4, IdxCat = 5,
                    Titre = "Yassine - Probleme serveur",
                    Probleme = "Yassine : serveur down pour toute l equipe."
                };

                for (int i = 0; i < cas.Length; i++)
                {
                    Scenario s = cas[i];
                    Client client = clients[s.IdxClient];
                    CategorieIncident cat = categories[s.IdxCat];

                    Console.WriteLine("---");
                    Console.WriteLine("client : " + client.Nom);

                    Materiel? mat = data.GetMateriel(client.ClientId);
                    if (mat != null)
                        Console.WriteLine("materiel : " + mat.Nom);

                    int idTicket = service.CreerTicket(client.ClientId, cat.CategorieId, s.Titre, s.Probleme);
                    data.AddAppel(client.ClientId, idTicket, s.Probleme);
                    Console.WriteLine("ticket " + idTicket + " - " + s.Titre);

                    bool fini = false;
                    int essais = 0;
                    while (!fini && essais < 8)
                    {
                        Ticket? tmp = tickets.GetById(idTicket);
                        if (tmp == null) break;

                        Console.WriteLine("-> " + data.GetTech(tmp.NiveauId));
                        service.TraiterTicket(idTicket);
                        data.AddIntervention(idTicket, tmp.NiveauId, "prise en charge");

                        tmp = tickets.GetById(idTicket);
                        if (tmp != null && (tmp.Statut == "Resolu" || tmp.Statut == "Non resolu"))
                            fini = true;
                        essais++;
                    }

                    if (i < cas.Length - 1)
                        Thread.Sleep(2000);

                    Console.WriteLine();
                }

                service.AfficherStats();
                Console.WriteLine("fin");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("erreur : " + ex.Message);
            }
        }
    }
}
