using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public class TONE
	{
		public M98 m98 = null;

		private int note;       //ノート
		private int octave;     //オクターブ
		private int qtz;        //クオンタイズ
		private int length;     //レングス
		private int volume;     //ボリューム
		private int voice;      //音色
		private int tie;        //タイ
		private int ptm;        //ポルタメント
		private int macro;      //macro
		private int[] pan = new int[2];        //panning
		private int ksift;      //key sift
		private int clock;      //クロック
		private int extend;     //連符の数
		public TONE extadr;

		public TONE(flag a, M98 m98)
		{
			this.m98 = m98;
		}

		public int getnte() { return note; }
		public int getoct() { return octave; }
		public int getqtz() { return qtz; }
		public int getlen() { return length; }
		public int getvol() { return volume; }
		public int getvoi() { return voice; }
		public int gettie() { return tie; }
		public int getptm() { return ptm; }
		public int getmac() { return macro; }
		public int getpan(int n) { return pan[n]; }
		public int getkst() { return ksift; }
		public int getclk() { return clock; }
		public int getext() { return extend; }

		public void putnte(int a) { note = a; }
		public void putoct(int a) { octave = a; }
		public void putqtz(int a) { qtz = a; }
		public void putlen(int a) { length = a; }
		public void putvol(int a) { volume = a; }
		public void putvoi(int a) { voice = a; }
		public void puttie(int a) { tie = a; }
		public void putptm(int a) { ptm = a; }
		public void putmac(int a) { macro = a; }
		public void putpan(int index,int a) { pan[index] = a; }
		public void putkst(int a) { ksift = a; }
		public void putclk(int a) { clock = a; }
		public void putext(int a) { extend = a; }

		public void mk_ext(int i)   //連符の作成
		{
			if (extend == 1 || length % extend != 0 || length <= gWork.LimitExCounter)
			{ extend = 1; return; }//連符なしならリターン

			int ln = length / extend;
			int y;
			int[] n = new int[DEF.MAXEXTEND], o = new int[DEF.MAXEXTEND], l = new int[DEF.MAXEXTEND];
			n[0] = note; o[0] = octave; l[0] = ln;
			for (y = 1; y < extend; y++)    //あらかじめ連符の内容を作っておく
			{
				m98.newrnte(ref n[y], ref o[y], i);
				if (m98.nao2key(n[y], o[y]) == m98.nao2key(n[y - 1], o[y - 1]))
				{//もし前と同じkeyだったら
					l[y - 1] += ln;
					extend--;
					y--;
				}
				else l[y] = ln;
			}

			length = l[0];
			TONE p, q;
			p = new TONE(flag.OFF, m98);
			extadr = p;
			for (y = 1; y < extend; y++)
			{
				q = new TONE(flag.OFF, m98);

				p.note = n[y];
				p.octave = o[y];
				p.length = l[y];
				if (p.qtz > 0 && p.qtz < 192) p.qtz = qtz / extend;
				else p.qtz = qtz;
				p.volume = volume;
				p.voice = voice;
				p.ptm = 0;
				p.tie = 0;
				p.macro = 0;
				p.pan = pan;
				p.clock = clock;
				p.ksift = ksift;
				p.extend = 1;
				p.extadr = q;
				p = q;
			}
		}

		public void puttone(int nt, int qt, int ln, int ot, int vl, int vi, int ti, int pt, int mc, int pa1, int pa2, int ks,int clk, int ex)
		{
			if (ln < gWork.wk.msl)
			{
				gWork.wk.msl = ln;
			}
			note = nt;
			qtz = qt;
			length = ln;
			octave = ot;
			volume = vl;
			voice = vi;
			tie = ti;
			ptm = pt;
			macro = mc;
			pan[0] = pa1;
			pan[1] = pa2;
			ksift = ks;
			clock = clk;
			extend = ex;
		}

		public void Copy(TONE tONE)
		{
			note = tONE.getnte();
			qtz = tONE.getqtz();
			length = tONE.getlen();
			octave = tONE.getoct();
			volume = tONE.getvol();
			voice = tONE.getvoi();
			tie = tONE.gettie();
			ptm = tONE.getptm();
			macro = tONE.getmac();
			pan[0] = tONE.getpan(0);
			pan[1] = tONE.getpan(1);
			ksift = tONE.getkst();
			clock = tONE.getclk();
			extend = tONE.getext();
		}
	}
}
