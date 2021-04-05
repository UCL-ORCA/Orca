using Orca.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services
{
    public interface IEventAggregator
    {
        Task ProcessEvent(StudentEvent studentEvent);
        
    }
}
