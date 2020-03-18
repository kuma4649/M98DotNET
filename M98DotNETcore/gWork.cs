using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
    public static class gWork
    {
        public static Random random = new Random();
        public static grp_prm grp = new grp_prm();
        public static M98 m98 = null;
        public static command com = null;//	変換コマンド

        //------------------------------------------------------------------
        //ＭＭＬイニシャライズフラグ
        public static flag initf_qtz = flag.OFF;       //MMLのイニシャライズをするかどうかのflag
        public static flag initf_vol = flag.OFF;
        public static flag initf_voi = flag.OFF;
        public static flag initf_pan = flag.OFF;
        public static flag initf_kst = flag.OFF;
        public static flag initf_clk = flag.OFF;
        //------------------------------------------------------------------
        //グループフラグ
        public static int voigrf, lengrf = 0;
        public static int macgrf, ntegrf, qtzgrf = 0;
        public static int kstgrf, extgrf = 0;

        //------------------------------------------------------------------
        //バッファ、グローバル変数

        public static TONE[] orgt;     //	元の音符の構成データのワーク
        public static TONE[] cpyt;     //	変換された音符の構成データのワーク
        public static repval rp = new repval(0, flag.OFF);	//	バリューリピートカウンタ、フラグ
        public static attrib atr = new attrib(flag.OFF);    //	属性変数
        public static flag message_flag = flag.OFF;

        //FILE* fi,*fi2,*fo,*fo2;
        public static string sndtxt;       //	読み込んだテキストのバッファ
        public static string cpytxt;       //	変換後のコピー用バッファ
        public static string sndbuf1;      //	１行あたりの変換用バッファ
        public static string sndbuf2;      //	１行あたりの変換用バッファ2
        public static string subbuf;       //	リマーク(;)以後の要素格納用
        public static string allcombf;     //	変換コマンドの総括バッファ
        public static int[] progress;      //	数列バッファ
        public static int[] cpyprg;        //	変換用数列バッファ

        public static int mst = 0;
        public static int rotatedat = 0;
        public static int rndper = 100;       //ランダムパーセンテージ
        public static int randmin = 0, randmax = 0;
        public static int comline = 0;
        public static int hismac = 0;
        public static int TNUM = 0;           //音符の数が入る
        public static int PTMMODE = 0;        //ポルタメントモード
        public static int LimitExCounter = 0; //連符にできる長さの上限値

        public static bool preserve = false;
        public static flgdat preserveFlags = null;
        public static TONE preserveTone = null;

        public static string[] notedat = new string[12] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

        public static linedat[] mml;

        public static compw wk = new compw(0, 4, 6, 255, 96, DEF.EMPTY, DEF.EMPTY, DEF.EMPTY, DEF.EMPTY, DEF.EMPTY);


        public static void setup(M98 m98)
        {
            gWork.m98 = m98;
            com = new command(flag.ON, m98);
        }
    }
}
