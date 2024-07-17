using API.Services.Interfaces;
using Models;

namespace API.Services.Implement
{
    public class ComboSvc : IAddable<Combo>, IEditable<Combo>, IDeletable<Combo>, IReadable<Combo>, ILookup<Guid,  Combo>, Guid<string, Combo>
    {
    }
}
