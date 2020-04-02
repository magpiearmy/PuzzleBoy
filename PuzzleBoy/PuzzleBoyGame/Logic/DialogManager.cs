using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class DialogManager
    {
        private Dialog _dialog;
        private Game _game;

        public DialogManager(Game game)
        {
            _game = game;
        }

        public Dialog GetDialog()
        {
            return _dialog;
        }

        public void InitDialog(String text)
        {
            _dialog = new Dialog(_game.Content, text);
            _dialog.Init();
            _dialog.SetText(text);
        }
    }
}
