using System.Threading.Tasks;

namespace iss_data.Services.Face
{
    public interface IUpstreamSender
    {
        void SendMessage(string message);

        Task Flush();
    }
}