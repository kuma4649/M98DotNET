using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public class command
	{
		public string[] CommandName = new string[]
		{
				"rv","mr","rmr","crand","rand",
				"rr","rl","rfil","rep","@grp",
				"lgrp","mgrp","ngrp","qgrp","kgrp",
				"@grpl","mgrpl","init","mask","maskl",
				"select","selectl","egrp","sift","racc",
				"maskext","PTMMODE","call","mcopy","DISABLE",
				"ENABLE","limite","preserve","","",
				"","","","",""
		};

		public flag MCOPY_FLAG;//mcopy
		public flag SIFT_FLAG;//sift
		public flag RAND_FLAG;//rand
		public flag CRAND_FLAG;//crand
		public flag CALL_FLAG;//call
		public flag RV_FLAG;//reverse
		public flag MR_FLAG;//mirror
		public flag RMR_FLAG;//r.mirror
		public flag RR_FLAG;//rotate right
		public flag RL_FLAG;//rotate left
		public string name = "";//コマンド名
		public string[] sptr = new string[DEF.MAXCOMP];//ストリングパラメータのポインタ
		public int[] prm = new int[DEF.MAXCOMP];//コマンドパラメータデータ
		public int spnum;//ストリングパラメータの数
		public int pnum;//コマンドパラメータの数
		public M98 m98;

		public command(flag f, M98 m98)
		{
			this.m98 = m98;

			allflgis(f);
			name = "";//, '\0', DEF.MAXCOMNAME);
			for (int i = 0; i < DEF.MAXCOMP; i++) prm[i] = 0;//, DEF.MAXCOMP);
			spnum = pnum = 0;
		}

		public void on_off(string com, flag f)
		{

			if (com == "mcopy") MCOPY_FLAG = f;
			if (com == "sift") SIFT_FLAG = f;
			if (com == "rand") RAND_FLAG = f;
			if (com == "crand") CRAND_FLAG = f;
			if (com == "call") CALL_FLAG = f;
			if (com == "rv") RV_FLAG = f;
			if (com == "mr") MR_FLAG = f;
			if (com == "rmr") RMR_FLAG = f;
			if (com == "rr") RR_FLAG = f;
			if (com == "rl") RL_FLAG = f;

			if (com == "ALL") allflgis(f);

		}

		public void mcopy()
		{
			if (MCOPY_FLAG == flag.ON) mk_mcopy();
		}

		public void sift()
		{
			if (SIFT_FLAG == flag.ON) siftprg();
		}

		public void rand()
		{
			if (RAND_FLAG == flag.ON) mk_rdprg(gWork.TNUM);
		}

		public void crand()
		{
			if (CRAND_FLAG == flag.ON) mk_crprg(gWork.TNUM);
		}

		public void call(ref string a, ref int aPtr)
		{
			if (CALL_FLAG == flag.ON) mg_com(ref a, ref aPtr, prm[0]);
		}

		public void rv()
		{
			if (RV_FLAG == flag.ON) mk_rvprg(gWork.TNUM);
		}

		public void mr()
		{
			if (MR_FLAG == flag.ON) mk_mrprg(gWork.TNUM);
		}

		public void rmr()
		{
			if (RMR_FLAG == flag.ON) mk_rmrprg(gWork.TNUM);
		}

		public void rr()
		{
			if (RR_FLAG == flag.ON) mk_rsprg(gWork.TNUM);
		}

		public void rl()
		{
			if (RL_FLAG == flag.ON) mk_lsprg(gWork.TNUM);
		}

		public void allflgis(flag f)
		{
			MCOPY_FLAG = f; //mcopy
			SIFT_FLAG = f;  //sift
			RAND_FLAG = f;  //rand
			CRAND_FLAG = f; //crand
			CALL_FLAG = f;  //call
			RV_FLAG = f;    //reverse
			MR_FLAG = f;    //mirror
			RMR_FLAG = f;   //r.mirror
			RR_FLAG = f;    //rotate right
			RL_FLAG = f;    //rotate left
		}

		public int getnumber(string comstring)
		{
			for (int i = 0; i < CommandName.Length; i++)
			{
				if (CommandName[i] == comstring)
					return i;
			}
			return DEF.ERROR;     //コマンドがない
		}

		//------------------------------------------------------------------
		//マクロコピー
		private void mk_mcopy()
		{
			int l = Common.searchm(gWork.com.prm[0]);
			if (l == DEF.ERROR)
			{
				DEF.PF(string.Format(
						"\n指定されたマクロナンバ({0})がない、またはマクロ行ではありません"
						, gWork.com.prm[0]
						));
				throw new M98Exception();
			}
			Common.lnkdat(l, l + 1);   //set MML to sndbuf1
			if (gWork.hismac != 0)
			{   //コピー先がマクロ行なら
				int tmp = gWork.hismac;
				m98.settone();
				gWork.hismac = tmp;
			}
			else
			{
				DEF.PF("\nコピー先がマクロでない");
				throw new M98Exception();
			}
			Common.initprg();
			m98.initcpyt(gWork.TNUM);
		}

		//------------------------------------------------------------------
		//各要素のシフト
		private void siftprg()
		{
			if (gWork.com.pnum == 0) return;
			//qsort(com.prm,com.pnum,sizeof(int),		//com.prmを小から昇順に
			//(int(*)(const void *,const void *)) cmpi);	//クイックソートする	
			//int smin=com.prm[0];int smax=com.prm[1];int sabun=0;
			//if(smin<0){sabun=0-smin;smin=smin+sabun;smax=smax+sabun;}

			for (int i = 0; i <= gWork.TNUM; i++)
			{
				if (gWork.atr.fnte == flag.ON && gWork.cpyt[i].getnte() != DEF.REST)
				{           //ノートの絶対シフト
							//ノート部分
					int n = gWork.cpyt[i].getnte();
					int o = gWork.cpyt[i].getoct();
					int k = m98.nao2key(n, o);
					//k=k+((random((smax-smin)+1)+smin)-sabun);
					int rs = gWork.com.prm[gWork.random.Next(gWork.com.pnum)];
					k = k + rs;
					if (k > 95 || k < 0) k = 36;
					m98.key2nao(k, out n, out o);
					gWork.cpyt[i].putnte(n);
					gWork.cpyt[i].putoct(o);
					//ポルタメント部分
					k = rs;
					k += gWork.cpyt[i].getptm();
					if (k > 95 || k < 0) gWork.cpyt[i].putptm(0);//ポルタメント消去
					else gWork.cpyt[i].putptm(k);
					//連符部分
					if (gWork.cpyt[i].getext() != 1)
					{
						TONE p;
						p = gWork.cpyt[i].extadr;
						for (int y = 1; y < gWork.cpyt[i].getext(); y++)
						{
							n = p.getnte();
							o = p.getoct();
							k = m98.nao2key(n, o);
							k += gWork.com.prm[gWork.random.Next(gWork.com.pnum)];
							if (k > 95 || k < 0) k = 36;
							m98.key2nao(k, out n, out o);
							p.putnte(n);
							p.putoct(o);
							p = p.extadr;
						}
					}
				}
				if (gWork.atr.fkst == flag.ON) //キーシフトの絶対シフト
				{
					gWork.cpyt[i].putkst(gWork.cpyt[i].getkst() + gWork.com.prm[gWork.random.Next(gWork.com.pnum)]);
				}
				if (gWork.atr.fmac == flag.ON) //マクロの絶対シフト
				{
					gWork.cpyt[i].putmac(gWork.cpyt[i].getmac() + gWork.com.prm[gWork.random.Next(gWork.com.pnum)]);
				}
			}
		}

		//------------------------------------------------------------------
		//0からxまでの完全乱数数列を作る
		private void mk_rdprg(int x)
		{
			gWork.rndper = 100;//KUMA:必ず100%に初期化する
			if (gWork.com.pnum > 0) gWork.rndper = gWork.com.prm[0];    //乱数確率を得る

			int i;
			int y = -1;
			for (i = 0; i <= x + 1; i++)
			{
				gWork.atr.putall(i, atrb.INIT);    //属性初期化

			loop:
				int rdf = 1;
				int a = gWork.random.Next(x + 1);
				if (gWork.rndper != 100 && gWork.random.Next(100) > gWork.rndper - 1 && i != x + 1)
				{
					a = gWork.cpyprg[i];
					rdf = 0;
				}

				if (i != 0 && i != x + 1)
				{
					for (y = 0; y <= i; y++)
					{
						if (x == y && y == a) { break; }
						if (gWork.progress[y] == a && y != i) goto loop;
					}
				}
				if (i != x + 1)
				{
					if (rdf != 0) gWork.atr.putall(i, atrb.RAND);//完全乱数属性
					gWork.progress[i] = a;
				}
			}

		}

		//------------------------------------------------------------------
		//0からxまでの循環乱数を作る
		private void mk_crprg(int x)
		{
			gWork.rndper = 100;//KUMA:必ず100%に初期化する
			if (gWork.com.pnum > 0) gWork.rndper = gWork.com.prm[0];    //乱数確率を得る
			int i;
			int y = -1;
			for (i = 0; i <= x + 1; i++)
			{
				gWork.atr.putall(i, atrb.INIT);    //属性初期化

			loop:
				int a = gWork.random.Next(x + 1);
				if (gWork.rndper != 100 && gWork.random.Next(100) > gWork.rndper - 1 && i != x + 1)
				{
					a = gWork.cpyprg[i];
				}

				if (i != 0 && i != x + 1)
				{
					for (y = 0; y <= i; y++)
					{
						if (x == y && y == a) { break; }
						if (gWork.progress[y] == a && y != i) goto loop;
					}
				}
				if (i != x + 1)
				{
					gWork.atr.putall(i, atrb.CRAND);//循環乱数属性
					gWork.progress[i] = a;
				}
			}

		}

		//------------------------------------------------------------------
		//コマンドマージ
		//	mgadrから始まるコマンド文字列に、コマンドグループcomgの内容
		//	をコマンドバッファにマージする
		private void mg_com(ref string mgadr, ref int mgPtr, int comg)
		{
			int coms = 0, mmls = 0;
			bool found = false;
			string pushbuf = "";  //一時待避用バッファ
			int i = 0;

			for (i = mgPtr; i < mgadr.Length; i++)
			{
				pushbuf += mgadr[i].ToString();
			}

			if (null == pushbuf)//mgadr以降のコマンドを待避
			{
				DEF.PF("\nコマンドバッファオーバーフロー");
				throw new M98Exception();
			}

			for (i = 0; i < gWork.mml.Length; i++)
			{
				if (gWork.mml[i].chheader == "{")
				{
					int p = 0;
					while (p < gWork.mml[i].mmldat.Length && gWork.mml[i].mmldat[p] == ' ') { p++; }
					if (Common.atoi(gWork.mml[i].mmldat, p) == comg)
					{
						coms = i + 1;
						mmls = searche(i);
						found = true;
						break;
					}
				}
			}
			if (!found)
			{
				DEF.PF(string.Format("\n{0}<- 指定されたコマンドグループは存在しません", comg));
				throw new M98Exception();
			}
			mgadr = mgadr.Substring(0, mgPtr);
			setcomb(coms, mmls, ref mgadr); //新たにmgadrからマージする
			mgadr += pushbuf;
			//gWork.allcombf += pushbuf;  //待避していた文字列をもどす
			//free(pushbuf);
		}

		//------------------------------------------------------------------
		//指定されたバッファbufferにcoms行からmmls-1行までのコマンドをセットする

		private void setcomb(int coms, int mmls, ref string buffer)
		{
			int i = 0;
			while (coms != mmls)
			{
				string a = gWork.mml[coms].mmldat;
				int aPtr = 0;
				while (aPtr < a.Length && a[aPtr] != '\n')
				{
					if (a[aPtr] == ';')  //コマンドライン中のリマーク
					{
						while (aPtr < a.Length && a[aPtr] != '\n') { aPtr++; }
					}
					else
					{
						if (a[aPtr] == ' ' || a[aPtr] == '\t')
						{
							aPtr++;
						}
						else
						{
							buffer += a[aPtr++];
							i++;
							if (i > DEF.MAXCOMBF * 1024)
							{
								DEF.PF("\nコマンドバッファオーバー");
								throw new M98Exception();
							}
						}
					}
				}
				coms++;
			}
			//*buffer = '\0';
		}

		//------------------------------------------------------------------
		/*
			n行の'{'と対になっている'}'の行番号を得る
			戻り値：行番号
		*/
		private int searche(int n)
		{
			n++;
			while (gWork.mml[n].chheader != "}")
			{
				if (gWork.mml[n].chheader == "{" || gWork.mml[n].chheader == "")
				{
					DEF.PF("\nError:｛の後に｝がありません");
					throw new M98Exception();
				}
				n++;
			}
			return (n);
		}

		//------------------------------------------------------------------
		//0からxまでの数列のリバース変換を行う。
		private void mk_rvprg(int x)
		{
			int y = 0;
			for (int i = 0; i <= x; i++)
			{
				y = gWork.cpyprg[x - i];
				gWork.progress[i] = y;
				gWork.atr.putall(i, atrb.Reverse); //リバース属性
			}
		}

		//------------------------------------------------------------------
		//0からxまでの数列のリバースミラー変換を行う。
		private void mk_rmrprg(int x)
		{
			int y = 0;
			for (int i = 0; i <= x; i++)
			{
				y = gWork.progress[x - i];
				gWork.progress[i] = y;
				gWork.atr.putall(i, atrb.REVMIRROR);   //リバースミラー属性
			}
		}

		//------------------------------------------------------------------
		//0からxまでの数列のミラー変換を行う。
		private void mk_mrprg(int x)
		{
			int y = 0;
			for (int i = 0; i <= x; i++)
			{
				y = gWork.progress[i];
				gWork.progress[x - i] = y;
				gWork.atr.putall(i, atrb.MIRROR);  //ミラー属性
			}
		}

		//------------------------------------------------------------------
		//0-xの数列を右へrotatedat回ローテートする
		private void mk_rsprg(int x)
		{
			gWork.rotatedat = gWork.com.prm[0];       //ローテート係数の取得
			int a, i, y = 0;
			int n = gWork.rotatedat;
			if (x + 1 >= n) { a = (x + 1) - n; }
			else a = (x + 1) - (n % (x + 1));
			for (i = a; i <= x; i++)
			{
				gWork.atr.putall(y, atrb.ROTATER); //右ローテート属性
				gWork.progress[y] = gWork.cpyprg[i]; y++;
			}
			if (a != 0)
			{
				for (i = 0; i <= a - 1; i++)
				{
					gWork.atr.putall(y, atrb.ROTATER);
					gWork.progress[y] = gWork.cpyprg[i]; y++;
				}
			}
		}

		//------------------------------------------------------------------
		//0-xを左へn回ローテートする
		private void mk_lsprg(int x)
		{
			gWork.rotatedat = gWork.com.prm[0];       //ローテート係数の取得
			int a, i, y = 0;
			int n = gWork.rotatedat;
			if (x >= n) { a = n; }
			else a = (n % (x + 1));
			for (i = a; i <= x; i++)
			{
				gWork.atr.putall(y, atrb.ROTATEL); //左ローテート属性
				gWork.progress[y] = gWork.cpyprg[i]; y++;
			}
			if (a != 0)
			{
				for (i = 0; i <= a - 1; i++)
				{
					gWork.atr.putall(y, atrb.ROTATEL);
					gWork.progress[y] = gWork.cpyprg[i]; y++;
				}
			}

		}

	}
}
