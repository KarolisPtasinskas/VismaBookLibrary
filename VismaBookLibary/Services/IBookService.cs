namespace VismaBookLibary.Services
{
    public interface IBookRepository
    {
        string Add(string bookJson);
        string Delete(string delteDTOJson);
        string GetAll();
        string GetFiltered(string filterDTOJson);
        string Return(string returnDTOJson);
        string Take(string borrowDTOJson);
    }
}