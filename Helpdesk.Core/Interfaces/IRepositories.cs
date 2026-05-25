using System.Collections.Generic;
using Helpdesk.Core.Models;

namespace Helpdesk.Core.Interfaces
{
    public interface ITicketRepository
    {
        int Create(Ticket ticket);
        Ticket? GetById(int id);
        void Update(Ticket ticket);
        List<Ticket> GetAll();
        void AddHistorique(int ticketId, string action);
    }

    public interface IBaseConnaissanceRepository
    {
        void Add(BaseConnaissance article);
        List<BaseConnaissance> GetByCategorie(int categorieId);
    }
}
