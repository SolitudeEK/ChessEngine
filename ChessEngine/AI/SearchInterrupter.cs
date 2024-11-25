using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.AI
{
    public class SearchInterrupter
    {
        private static SearchInterrupter _instance;
        private bool _halt;

        private SearchInterrupter()
        {
            _halt = false;
        }

        public static SearchInterrupter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SearchInterrupter();
                }
                return _instance;
            }
        }

        public void Interrupt()
        {
            _halt = true;
        }

        public void Resume()
        {
            _halt = false;
        }

        public bool Interrupted => _halt;
    }
}
