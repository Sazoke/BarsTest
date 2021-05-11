using Reversi.Models;

namespace Reversi.Repositories
{
    public interface IGameRepository
    {
        bool IsPlayer(int id);
        int SetPlayer(int id);
        int GetPlayer(int id);
        bool IsEnd();
        void End();
        bool HavePlace();
        bool HaveChange();
        public bool IsYourChange(int id);
        GameChange GetChange();
        void SetChange(GameChange gameChange);
    }
}