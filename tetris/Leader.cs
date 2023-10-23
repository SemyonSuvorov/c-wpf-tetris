using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tetris
{
    internal class Leader
    {
        public int Score { get; }
        public string Name { get; }
        public Leader(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
