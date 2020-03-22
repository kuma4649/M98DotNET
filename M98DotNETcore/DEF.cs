using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
    public static class DEF
    {
        public const int MAXTONE = 1024;    //一度に変換できる音符の最大数
        public const int MAXTXT = 16;   //Text Buffer(KB)
        public const int MAXBUF = 16;   //変換作業用バッファサイズ(KB)
        public const int MAXPRG = 4;    //max=2048(KB)
        public const int MAXCOMBF = 4;  //変換コマンド用バッファ
        public const int MAXCOMNAME = 20;   //コマンドネームの長さ
        public const int DUMMY = -1;    //マクロを使用するためのノートのダミー
        public const int REST = -2; //休符データ
        public const int MAXVOIGR = 127;    //音色グループ数
        public const int MAXLENGR = 127;    //レングスグループ数
        public const int MAXQTZGR = 127;    //クオンタイズグループ数
        public const int MAXNTEGR = 127;    //ノートグループ数
        public const int MAXKSTGR = 127;    //キーシフトグループ数
        public const int MAXEXTGR = 127;    //連符グループ数
        public const int MAXPANGR = 127;    //PANグループ数
        public const int MAXMACGR = 255;    //マクログループ数
        public const int MAXCOMP = 30;  //コマンドパラメータ数
        public const int MAXEXTEND = 192 + 1;   //連符の最大値
        public const int NO_PTM = 0;    //ﾎﾟﾙﾀﾒﾝﾄが無い
        public const int NewTxt = 1;    //text save関数用
        public const int COMEND = -2;
        public const int ERROR = -32767;
        public const int NotNote = -32767;
        //public  const int		ERREND		=errsub()
        public const int EMPTY = -32767;
        //public  const int		MES		    =printf
        public const int ESC = 0x1b;

        //public static void ERREND()
        //{
        //    throw new M98Exception();
        //}

        public static void PF(string msg) {
            musicDriverInterface.Log.WriteLine(musicDriverInterface.LogLevel.INFO, msg);
            //Console.Write(msg); 
        }

        public static int CursorLeft { 
            set {
                //Console.CursorLeft = value;
            } 
        }

        internal static void SetCursorPosition(int x, int y)
        {
            //Console.SetCursorPosition(x, y);
        }
    }
}
