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

        public Player this[string playerNoOrToken]
        {
            get
            {
                var no = stringKeyToNo(playerNoOrToken);

                var player = this[no];

                if (player != null)
                {
                    return player;
                }

                return this.FirstOrDefault(p => p.Token == playerNoOrToken);
            }
            set
            {
                if (int.TryParse(playerNoOrToken, out int no))
                {
                    this[no] = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Value has to be an integer but is '{playerNoOrToken}'");
                }
            }
        }

        public Player this[int playerNo]
        {
            get
            {
                if (playerNo <= 0 || playerNo > _player.Length)
                {
                    return null;
                }
                return _player[playerNo - 1];
            }
            set
            {
                _player[playerNo - 1] = value;
            }
        }

        private int stringKeyToNo(string playerNo)
        {
            if (int.TryParse(playerNo, out int no))
            {
                return no;
            }

            return -1;
        }

        public List<Player> AllExcept(Player player)
        {
            return _player.Where(p => player == null || p != player).ToList();
        }
    }
}