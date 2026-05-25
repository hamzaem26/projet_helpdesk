Mon projet Helpdesk

C# console + postgres. C'est pour gerer des tickets support.

Ca fait quoi :
un client appelle, on cree un ticket, le support niveau 1/2/3 essaie de regler.
Ya aussi les appels, interventions, materiel, historique et base de connaissances.

Projets :
App = Program.cs (demo)
Core = HelpdeskService
Data = BDD
Tests = MSTest

Pour lancer :
1) postgres allume
2) base helpdesk_db
3) mdp dans Program.cs ligne connexion
4) dotnet run --project Helpdesk.App

Base de donnees :
J'ai utilise PostgreSQL en local sur mon pc (localhost port 5432).

j'ai travaillé avec pgAdmin comme une interface graphique .
La base s'appelle helpdesk_db.

DbInitializer cree les tables et remplit les donnees (clients, techniciens, categories...).
A chaque run ca vide les tickets et remet les donnees de base.

Pour les tests :
dotnet test Helpdesk.Tests
(postgres allume pour les tests bdd)

DbInitializer cree les tables tout seul.

Demo avec Geoffroy Michael Hamza Sarah Yassine.

Fichiers a regarder : Program.cs, HelpdeskService.cs, DbInitializer.cs

SQL :
select * from tickets;
select * from appels;
select * from interventions;
