using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doppelkopf.Core.App
{
    public class PlayerHolder : IEnumerable<Player>
    {
        private Player[] _player = new Player[4];

        public PlayerHolder()
        {
            for (int i = 1; i <= 4; i++)
            {
                this[i] = new Player(i);
            }
        }

        private int s2i(string playerNo)
        {
            if (int.TryParse(playerNo, out int _) == false)
            {

            }
            return Convert.ToInt32(playerNo) - 1;
        }

        public IEnumerator<Player> GetEnumerator()
        {
            for (int i = 0; i < 4; i++)
            {
                yield return _player[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Player this[string playerNo]
        {
            get
            {
                return _player[s2i(playerNo)];
            }
            set
            {
                _player[s2i(playerNo)] = value;
            }
        }

        public Player this[int playerNo]
        {
            get
            {
                return _player[playerNo - 1];
            }
            set
            {
                _player[playerNo - 1] = value;
            }
        }

        public List<Player> AllExcept(Player player)
        {
            return _player.Where(p => player == null || p != player).ToList();
        }
    }
}
