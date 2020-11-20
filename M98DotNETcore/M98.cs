using musicDriverInterface;
using System;
using System.IO;

namespace M98DotNETcore
{
	public class M98 : iPreprocessor
	{
		public int? seed { get; set; } = null;

		public M98(iEncoding enc = null)
		{
			this.enc = enc ?? myEncoding.Default;
		}

		public void Preprocess(Stream sourceMML, Stream destMML, Func<string, Stream> appendFileReaderCallback)
		{
			string srcMML = enc.GetStringFromSjisArray(ReadAllBytes(sourceMML));
			string dstMML;

			Main(srcMML, out dstMML, seed);

			var ary = enc.GetSjisArrayFromString(dstMML);
			destMML.Write(ary, 0, ary.Length);
		}

		public void Init()
		{
		}

		public PreprocessorInfo GetPreprocessorInfo()
		{
			return null;
		}




		private iEncoding enc = null;

		/// <summary>
		/// ストリームから一括でバイナリを読み込む
		/// </summary>
		private byte[] ReadAllBytes(Stream stream)
		{
			if (stream == null) return null;

			var buf = new byte[8192];
			using (var ms = new MemoryStream())
			{
				while (true)
				{
					var r = stream.Read(buf, 0, buf.Length);
					if (r < 1)
					{
						break;
					}
					ms.Write(buf, 0, r);
				}
				return ms.ToArray();
			}
		}

		private void Main(string sndtxt, out string cpytxt, int? seed)
		{
			gWork.setup(this);
			openmem();      //メモリ確保
			randomize(seed);        //乱数初期化！

			gWork.sndtxt = sndtxt;
			gWork.cpytxt = "";

			//行ごとにMMLデータのポインタをセット
			new MMLDAT(gWork.sndtxt, ref gWork.mml);
			//変換作業のメイン関数
			replace();
			cpytxt = gWork.cpytxt;

			closemem();
		}

		private void randomize(int? seed)
		{
			if (seed == null) gWork.random = new Random();
			else gWork.random = new Random((int)seed);
		}

		//----------------------------------------------------
		//メモリを確保するよ
		private void openmem()
		{
			gWork.orgt = new TONE[DEF.MAXTONE];
			gWork.cpyt = new TONE[DEF.MAXTONE];
			for (int i = 0; i < DEF.MAXTONE; i++)
			{
				gWork.orgt[i] = new TONE(flag.OFF, this);
				gWork.cpyt[i] = new TONE(flag.OFF, this);
			}
			gWork.cpytxt = "";// getmemcp(DEF.MAXTXT);
			gWork.subbuf = "";// getmemcp(MAXBUF);
			gWork.sndbuf1 = "";// getmemcp(MAXBUF);
			gWork.sndbuf2 = "";// getmemcp(MAXBUF);
			gWork.allcombf = "";// getmemcp(MAXCOMBF);
			gWork.progress = new int[DEF.MAXPRG * 1024];
			gWork.cpyprg = new int[DEF.MAXPRG * 1024];
		}

		//----------------------------------------------------
		//メモリを解放するよ
		private void closemem()
		{
			gWork.sndbuf1 = null;
			gWork.sndbuf2 = null;
			gWork.cpytxt = null;
			gWork.allcombf = null;
			gWork.orgt = null;
			gWork.cpyt = null;
			gWork.progress = null;
			gWork.cpyprg = null;
		}

		//------------------------------------------------------------------
		//新しい乱数ノートを設定する
		internal void newrnte(ref int nt, ref int ot, int i)
		{
			int y = i - 1;
			if (i == 0) y = 0;
			if (gWork.ntegrf != 0)
			{       //もしノートグループが設定されてたら
				if (i != 0 && gWork.rp.c_nte != 0)
				{
					nt = gWork.cpyt[y].getnte();
					ot = gWork.cpyt[y].getoct();
					gWork.rp.c_nte--;
				}
				else
				{
					int n, o;
					key2nao(gWork.grp.getnte(gWork.random.Next(gWork.ntegrf)), out n, out o);
					nt = n;
					ot = o;
					gWork.rp.s_rep(); //counter initiarize
				}
			}
			else
			{
				if (i != 0 && gWork.rp.c_nte != 0)
				{
					nt = gWork.cpyt[y].getnte();
					ot = gWork.cpyt[y].getoct();
					gWork.rp.c_nte--;
				}
				else
				{
					int z = rndfil(85);
					int n, o;
					key2nao(z, out n, out o);
					nt = n;
					ot = o;
					gWork.rp.s_rep(); //couter initiarize
				}
			}

		}

		//-----------------------------------------------------------------
		//note&octave->key , key->note&octaveの変換

		internal int nao2key(int nte, int oct)       //Octave and note to Key
		{
			return ((oct - 1) * 12 + nte);
		}

		internal void key2nao(int key, out int nte, out int oct)   //Key to Octave and note
		{
			nte = key % 12;
			oct = (key / 12) + 1;
		}

		//-----------------------------------------------------------------
		//クロック値からリズム値のキャラクタ変換を行う
		internal void getlchr(int x, ref string pt)
		{
			pt = "";
			int cl = gWork.wk.clk / x;
			int mod = gWork.wk.clk % x;
			if (mod == 0)
			{
				pt = "l";
				pt += cl;
			}
			else    //付点音符かチェック
			{
				//KUMA:付点♪かどうか調べる
				int clkw = gWork.wk.clk;
				int count = 1;
				bool found = false;
				while (clkw > 0)
				{
					if (clkw + (clkw / 2) == x)//付点かな
					{
						found = true;
						break;
					}
					else if (clkw + (clkw / 2) < x)
					{
						break;
					}
					clkw /= 2;
					count *= 2;
				}

				if (found)
				{
					pt = string.Format("l{0}.", count);
				}
				else
				{
					pt = string.Format("l%{0}", x);
				}

				//if (mod / (x - mod) == 2)
				//{
				//	pt = "l";
				//	pt += (gWork.wk.clk / mod);
				//	pt += ".";
				//}
				//else
				//{
				//	pt = "l%";
				//	pt += x;
				//}
			}
		}

		//------------------------------------------------------------------
		/*
			乱数フィルタを用いた乱数の設定
			0->(r-1)の乱数のなかからrandmin%以上randmax%以下のものを選ぶ
		*/
		private int rndfil(int r)
		{
			//double rm, rs;
			//int z = 0;
			//if (gWork.randmax > 100) { gWork.randmax = 100; }
			//if (gWork.randmax < gWork.randmin) { gWork.randmax = 100; gWork.randmin = 0; }
			//if (gWork.randmax > 0) { rm = (r * gWork.randmax) / 100; } else { rm = 0; }
			//if (gWork.randmin > 1) { rs = ((r - 1) * gWork.randmin) / 100; } else { rs = 0; }
			//if (rm > 0)
			//{
			//	do
			//	{ z = (gWork.random.Next((int)rm)); }
			//	while (z < rs);
			//}
			//return (z);

			//指定(r)値が最小値より小さい場合は最小値を返して処理終了
			if (r < gWork.randmin) return gWork.randmin;

			//予め指定(r)値から最小値を引き、変化の大きさを求める  --(1)
			r -= gWork.randmin;
			//最大値から最小値を引き、変化の大きさを求める  --(2)
			int range = (gWork.randmax + 1) - gWork.randmin;
			//(1)のほうが(2)より大きい場合は、(2)を採用する
			if (r > range) r = range;

			//乱数を取得し、最小値を足す => 結果は最小値から最大値の範囲の乱数となる
			return gWork.random.Next(r) + gWork.randmin;

		}

		//private void gotoxy(int x, int y)
		//{
		//	DEF.SetCursorPosition(x, y);
		//}

		//private void cprintf(string msg)
		//{
		//	DEF.PF(msg);
		//}



		//----------------------------------------------------------------------------
		//----------------------------------------------------------------------------
		//				関数
		//----------------------------------------------------------------------------
		//----------------------------------------------------------------------------
		//指定領域のＭＭＬの変換作業

		private void replace()
		{
			//gotoxy(1, 4); cprintf("Now compile ...");
			string nowcpy = gWork.cpytxt;
			int x;
			int tn = 0;
			int preprocessCount = 0;
			while (tn < gWork.mml.Length)// && gWork.mml[tn].chheader != null)
			{
				if (gWork.mml[tn].chheader == "{")
				{
					preprocessCount++;
					x = searche(tn);            //'}'の行をさがす
					gWork.comline = tn;
					do
					{
						linecpy(ref nowcpy, tn);
						tn++;
					} while (gWork.mml[tn].chheader == null
							|| gWork.mml[tn].chheader == " "
							|| gWork.mml[tn].chheader == "\t");

					if (tn != x)
					{
						//{}内にMMLがあればそれを変換する
						DEF.CursorLeft = 0;
						DEF.PF(string.Format("Now compile ...{0}", tn));
						lnkcom(gWork.comline, tn);// set command to allcombf
						Common.lnkdat(tn, x);  //set MML to sndbuf1
						repsnd(tn); //'{'と'}'の間を変換
									//gWork.mml[tn].mmldat = gWork.mml[tn].chheader + " " + gWork.mml[tn].mmldat + "\r\n";
									//tn = x;
					}
				}

				linecpy(ref nowcpy, tn);
				tn++;
			}

			DEF.PF("\n");
			DEF.PF(string.Format("Preprocessed count ... {0}", preprocessCount));


			//*nowcpy = *mml[tn].chheader;    //終了の'\0'をかく

			gWork.cpytxt = nowcpy;
		}

		private void linecpy(ref string dest, int tn)
		{
			//bool addRem = false;
			//if (!string.IsNullOrEmpty(gWork.mml[tn].chheader))
			//{
			//addRem = true;
			//}

			int n = 2;
			while (gWork.mml[tn].mmldat.Length > 0 && n > 0)
			{
				char lc = gWork.mml[tn].mmldat[gWork.mml[tn].mmldat.Length - 1];
				if ((lc == '\n' && n == 2) || (lc == '\r' && n == 1))
				{
					gWork.mml[tn].mmldat = gWork.mml[tn].mmldat.Substring(0, gWork.mml[tn].mmldat.Length - 1);
					n--;
				}
				else
				{
					n = 0;
				}
			}

			dest +=
				//(addRem ? "\'" : "")
				//+
				(gWork.mml[tn].chheader == null ? "" : gWork.mml[tn].chheader)
				+ gWork.mml[tn].mmldat
				+ "\r\n";
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
				if (gWork.mml[n].chheader == "{" || gWork.mml[n].chheader == "\0")
				{
					DEF.PF("\nError:｛の後に｝がありません");
					throw new M98Exception();
				}
				n++;
			}
			return n;
		}

		//---------------------------------------------------------------------------
		//coms行からmmls-1行までの変換コマンドを総括してallcombfにいれる

		private void lnkcom(int coms, int mmls)
		{
			gWork.allcombf = "";// MAXCOMBF * 1024    //最初にバッファを初期化しておく
			setcomb(coms, mmls, ref gWork.allcombf);  //バッファにセット
		}

		//------------------------------------------------------------------
		//指定されたバッファbufferにcoms行からmmls-1行までのコマンドをセットする

		private void setcomb(int coms, int mmls, ref string buffer)
		{
			int i = 0;
			while (coms != mmls)
			{
				string buf = gWork.mml[coms].mmldat;
				int a = 0;
				while (a != buf.Length && buf[a] != '\n')
				{
					if (buf[a] == ';')  //コマンドライン中のリマーク
					{
						a = buf.Length;
						break;
					}
					else
					{
						if (buf[a] == ' ' || buf[a] == '\t')
						{
							a++;
						}
						else
						{
							buffer += buf[a++];
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
			'{'と'}'で括られた部分を'{'に続くコマンドにしたがって変換する
			（x行からy行までを変換）
		*/
		private void repsnd(int mmls)
		{

			//#define retry	goto extcom
			//#define flgof	goto flgoff

			int i = 0;

			PARTINIT();

			gWork.hismac = (gWork.mml[mmls].chheader == "#") ? 1 : 0;

			settone();      //構造体orgtにデータ,TNUMに音符の数をセット
			Common.initprg();      //数列バッファのinit
			initcpyt(gWork.TNUM);     //cpytのinit
			gWork.randmax = 100;
			gWork.randmin = 0; //乱数フィールドの初期化
			gWork.atr.allclr();       //DISABLE属性を除いてすべてクリア

			string a = gWork.allcombf;
			int aPtr = 0;
			string b;

			do
			{
				b = a;
				getcom(a, ref aPtr); //com.nameにｺﾏﾝﾄﾞ1つ,com.prmにそのパラメータ、
									 //com.pnumにパラメータの数が入る

				switch (gWork.com.getnumber(gWork.com.name))    //コマンドによる分岐処理
				{
					case 0:
						gWork.com.rv(); break;    //逆順変換
					case 1:
						gWork.com.mr(); break;    //ミラー変換
					case 2:
						gWork.com.rmr(); break;   //リバースミラー変換
					case 3:
						gWork.com.crand(); break; //循環乱数変換
					case 4:
						gWork.com.rand(); break;  //完全乱数変換
					case 5:
						gWork.com.rr(); break;    //右ローテート
					case 6:
						gWork.com.rl(); break;    //左ローテート
					case 7:
						setrfil(); goto flgoff;   //乱数フィルタ  
					case 8:
						setrep(); goto flgoff;// extcom;    //リピート設定		//KUMA:オリジナルではextcomでしたが意図しない挙動になるので変えています	
					case 9:
						s_grpvoi(); goto flgoff;  //音色グループ		
					case 10:
						s_grplen(); goto flgoff;  //レングスグループ		
					case 11:
						s_grpmac(); goto flgoff;  //マクログループ		
					case 12:
						s_grpnte(); goto flgoff;  //ノートグループ		
					case 13:
						s_grpqtz(); goto flgoff;  //ｸｵﾝﾀｲｽﾞｸﾞﾙｰﾌﾟ	
					case 14:
						s_grpkst(); goto flgoff;  //キーシフトグループ	
					case 15:
						s_voigrl(); goto flgoff;  //音色グループline		
					case 16:
						s_macgrl(); goto flgoff;  //マクログループline
					case 17:
						initmml(); goto flgoff;   //ＭＭＬ初期化設定
					case 18:
						setmsk(); goto flgoff;    //マスク指定
					case 19:
						setmskl(); goto flgoff;   //マスク指定line
					case 20:
						setslct(gWork.TNUM); goto flgoff;   //セレクト指定
					case 21:
						setslctl(gWork.TNUM); goto flgoff;  //セレクト指定line
					case 22:
						s_grpext(); goto flgoff;  //連符グループ	
					case 23:
						gWork.com.sift(); break;  //シフト	
					case 24:
						rndacc(); goto flgoff;    //ランダムアクセント	
					case 25:
						setmskext(); goto flgoff; //連符のマスク
					case 26:
						gWork.PTMMODE = gWork.com.prm[0][0]; break;    //ﾎﾟﾙﾀﾒﾝﾄﾓｰﾄﾞ
					case 27:
						gWork.com.call(ref a, ref aPtr); goto flgoff; //コマンドマージ(call)
					case 28:
						gWork.com.mcopy(); goto flgoff; //マクロコピー
					case 29:
						comswitch(flag.OFF); goto flgoff;
					case 30:
						comswitch(flag.ON); goto flgoff;
					case 31:
						gWork.LimitExCounter = gWork.wk.clk / gWork.com.prm[0][0]; goto flgoff;
					case 32://プリザーブ
						gWork.preserve = true;
						gWork.preserveFlags = gWork.atr.Copy();
						goto flgoff;
					case 33:
						s_grpPan(); goto flgoff;  //パングループ
					case DEF.ERROR:
						DEF.PF("\nコマンドが異常！->");
						DEF.PF(b);
						throw new M98Exception();
					default: break;
				}

				for (i = 0; i <= gWork.TNUM; i++)
				{
					gWork.cpyprg[i] = gWork.progress[i];
				}

				mk_fprg(gWork.TNUM);  //新たな構造体cpytを作る		

				for (i = 0; i <= gWork.TNUM; i++)
				{
					gWork.orgt[i].Copy(gWork.cpyt[i]);
				}

				gWork.randmax = 100;
				gWork.randmin = 0; //乱数フィールドの初期化

				gWork.atr.clr();      //MASK､DISABLE属性を除く
									  //すべての属性をクリア
				Common.initprg();//KUMA:数列バッファのinit

			flgoff:
				gWork.atr.allflgof();     //変換フラグクリア	
										  //extcom:;


			}
			while (aPtr < a.Length && a[aPtr] != '\0');

			ptm_rewrite();          //ポルタメントの書き換えif(PTMMODE>=1)
			rmk_tone(gWork.TNUM);         //cpytから新たにsndbuf2を作る
			gWork.mml[mmls].mmldat = gWork.sndbuf2; //sndbuf2は新たなMMLDATA

		}

		//---------------------------------------------------------------------
		private void ptm_rewrite()
		{
			if (gWork.PTMMODE == 0) return;
			for (int i = 0; i < gWork.TNUM; i++)
			{
				int ptk = 0;
				if (gWork.cpyt[i].getptm() != DEF.NO_PTM)
				{
					ptk = nextkey(gWork.cpyt, i);
					if (nao2key(gWork.cpyt[i].getnte(), gWork.cpyt[i].getoct()) != ptk)
					{
						gWork.cpyt[i].putptm(ptk);
						gWork.cpyt[i].puttie(1);
					}
					else
					{//後のkeyと同じならptmじゃない
						gWork.cpyt[i].putptm(DEF.NO_PTM);
						gWork.cpyt[i].puttie(1);
					}
				}
			}
		}

		internal void initcpyt(int n)
		{
			for (int i = 0; i <= n; i++)
			{           //全部の要素を初期コピー
				gWork.cpyt[i].Copy(gWork.orgt[i]);
			}
		}

		//------------------------------------------------------------------
		//次にくるkeyを調べる（ポルタメントのための）
		private int nextkey(TONE[] p, int i)
		{
			if (p[i].getext() <= 1)
			{   //もし連符がなかったら
				return (nao2key(p[i + 1].getnte(), p[i + 1].getoct()));
			}
			else
			{
				TONE q = p[i].extadr;
				return (nao2key(q.getnte(), q.getoct()));
			}
		}

		//-----------------------------------------------------------------
		//一つの変換作業をするためのinit
		private void PARTINIT()
		{
			gWork.LimitExCounter = 0; //連符にできる上限値
			gWork.rndper = 100;       //乱数パーセンテージのinit
			gWork.rotatedat = 0;      //ローテート係数のinit
			gWork.atr.allflgof();     //変換フラグinit
			for (int i = 0; i < DEF.MAXVOIGR; i++) gWork.grp.voi[i] = 0; //音色グループinit	
			for (int i = 0; i < DEF.MAXLENGR; i++) gWork.grp.len[i] = 0; //リズムグループinit	
			gWork.grp.clsnte();       //ノートグループinit	
			for (int i = 0; i < DEF.MAXQTZGR; i++) gWork.grp.qtz[i] = 0; //クオンタイズグループinit	
			for (int i = 0; i < DEF.MAXMACGR; i++) gWork.grp.mac[i] = 0; //マクログループinit	
			grfinit();      //各グループフラグinit
			initfoff();     //mml init flag を全部off
			gWork.wk.initcwk();       //コンパイラのワークをinit
			gWork.rp.min = gWork.rp.max = 0;    //リピート回数のmin,maxをinit
			gWork.rp.allcof();        //リピートカウンタinit
			gWork.rp.allfof();        //リピートフラグoff
			gWork.preserve = false; //preserve off
		}

		private void grfinit()
		{
			gWork.voigrf = gWork.lengrf = gWork.macgrf = gWork.ntegrf = gWork.qtzgrf = gWork.kstgrf = gWork.extgrf = 0;
		}

		private void initfoff()
		{
			gWork.initf_qtz = flag.OFF;
			gWork.initf_vol = flag.OFF;
			gWork.initf_voi = flag.OFF;
			gWork.initf_pan = flag.OFF;
			gWork.initf_kst = flag.OFF;
			gWork.initf_clk = flag.OFF;
		}

		//-------------------------------------------------------------------
		//構造体toneにMMLの１行分のデータをセットする
		//TNUMに音符の数－１が入る
		internal void settone()
		{
			string adr = gWork.sndbuf1;
			int adrPtr = 0;
			int next;

			if (gWork.hismac != 0)        //もしマクロ行なら
			{
				while (adrPtr < adr.Length && adr[adrPtr] != '*') { adrPtr++; }
				adrPtr++;
				gWork.hismac = getpara(adr, ref adrPtr);//ポインタをマクロナンバ以降へ動かすダミーコール
				if (gWork.hismac == DEF.ERROR)
				{
					DEF.PF("\nマクロコマンドが異常。マクロナンバかマクロの内容が定義されていません");
					throw new M98Exception();
				}
				gWork.hismac++;
				while (adrPtr < adr.Length && adr[adrPtr] != '{') { adrPtr++; }
				if (adrPtr >= adr.Length)
				{
					DEF.PF("\nマクロ構文の解析に失敗しました。");
					throw new M98Exception();
				}
				adrPtr++;
			}

			int i;
			for (i = 0; adrPtr < adr.Length && adr[adrPtr] != '\n'; i++)
			{
				if (gWork.hismac != 0 && adr[adrPtr] == '}')
				{
					break;
				}

				next = schtone(adr, adrPtr, i);
				if (next < 0)
				{
					break;
				}

				adrPtr = next;
			}
			gWork.TNUM = i - 1;
		}

		//---------------------------------------------------------------------------
		/*
			コマンドパラメータを返す
			戻り値：パラメーターが無い場合はエラーとして－１を返す
		*/
		private int getpara(string a, ref int p)
		{
			while (p < a.Length && a[p] == ' ') { p++; }
			int ans = Common.atoi(a, p);     //パラメータ用

			if (!Common.isdigit(a, p) && (p >= a.Length || (a[p] != '-' && a[p] != '+')))
			{
				return (DEF.ERROR);
			} //先頭が+,-,数字以外ならエラー

			while (p < a.Length && (Common.isdigit(a, p) || a[p] == '-' || a[p] == '+'))
			{
				p++;
			}

			return (ans);
		}

		//---------------------------------------------------------------------------
		//音符を１つ読み出し、各要素を返す
		//戻り値：次の音符のポインタ
		private int schtone(string adr, int adrPtr, int tnm)
		{
			int x;
			int ptmf = 0, endf = 0;
			int nt = gWork.wk.nte;
			int qt = gWork.wk.qtz;
			int ln = gWork.wk.len;
			int ot = gWork.wk.oct;
			int vl = gWork.wk.vol;
			int vi = gWork.wk.voi;
			int kst = gWork.wk.kst;
			int pa1 = gWork.wk.pan[0];
			int pa2 = gWork.wk.pan[1];
			int clk = gWork.wk.clk;

			int ptm = 0;
			int tief = 0;
			int mac = 0;

			int ex = 1;

			while (endf == 0 && (adrPtr < adr.Length && adr[adrPtr] != '\n'))
			{
				if (getkey(adr, ref adrPtr, ref nt, ref ot) != DEF.NotNote)
				{
					if (adrPtr < adr.Length && adr[adrPtr] != '%')
					{
						x = getpara(adr, ref adrPtr);
						if (x != DEF.ERROR)
						{
							ln = getlen(x);
							if (adrPtr < adr.Length && adr[adrPtr] == '.')
							{
								adrPtr++; ln = ln + (ln / 2);
							}
						}
						else
						{
							if (adrPtr < adr.Length && adr[adrPtr] == '.')
							{
								adrPtr++; ln = gWork.wk.len + (gWork.wk.len / 2);
							}
						}
					}
					else
					{
						adrPtr++;
						ln = getpara(adr, ref adrPtr);
					}
					endf = 1; //break;
				}
				if (endf == 0)
				{
					switch (adr[adrPtr])
					{
						case 'r':
							nt = DEF.REST;
							adrPtr++;
							if (adrPtr < adr.Length && adr[adrPtr] != '%')
							{
								x = getpara(adr, ref adrPtr);
								if (x != DEF.ERROR)
								{
									ln = getlen(x);
									if (adrPtr<adr.Length && adr[adrPtr] == '.')
									{
										adrPtr++;
										ln = ln + (ln / 2);
									}
								}
								else
								{
									if (adrPtr < adr.Length && adr[adrPtr] == '.')
									{
										adrPtr++;
										ln = gWork.wk.len + (gWork.wk.len / 2);
									}
								}
							}
							else
							{
								adrPtr++;
								ln = getpara(adr, ref adrPtr);
							}
							endf = 1;
							break;
						case 'q':
							adrPtr++;
							qt = getpara(adr, ref adrPtr);
							gWork.wk.qtz = qt;
							break;
						case 'l':
							adrPtr++;
							if (adrPtr < adr.Length && adr[adrPtr] != '%')
							{
								ln = getpara(adr, ref adrPtr);
								ln = getlen(ln);
								if (adrPtr < adr.Length && adr[adrPtr] == '.') { adrPtr++; ln = ln + (ln / 2); }
							}
							else
							{
								adrPtr++;
								ln = getpara(adr, ref adrPtr);
							}
							gWork.wk.len = ln;
							break;
						case '@':
							adrPtr++;
							x = getpara(adr, ref adrPtr);
							gWork.wk.voi = x;
							vi = x;
							break;
						case '>':
							adrPtr++;
							gWork.wk.oct++;
							ot++;
							break;
						case '<':
							adrPtr++;
							gWork.wk.oct--;
							ot--;
							break;
						case ')':
							adrPtr++;
							x = getpara(adr, ref adrPtr);
							if (x != DEF.ERROR)
							{
								vl += x;
							}
							else
							{
								vl++;
							}
							gWork.wk.vol = vl;
							break;
						case '(':
							adrPtr++;
							x = getpara(adr, ref adrPtr);
							if (x != DEF.ERROR)
							{
								vl -= x;
							}
							else
							{
								vl--;
							}
							gWork.wk.vol = vl;
							break;
						case 'C':
							adrPtr++;
							clk = getpara(adr, ref adrPtr);
							gWork.wk.clk = clk;
							break;
						case 'o':
							adrPtr++;
							ot = getpara(adr, ref adrPtr);
							gWork.wk.oct = ot;
							break;
						case 'v':
							adrPtr++;
							vl = getpara(adr, ref adrPtr);
							gWork.wk.vol = vl;
							break;
						case ' ':
							adrPtr++;
							break;
						case 'p':
							adrPtr++;
							pa1 = getpara(adr, ref adrPtr);
							gWork.wk.pan[0] = pa1;
							pa2 = DEF.EMPTY;
							adrPtr = SkipSpaceAndTab(adr, adrPtr);
							if (adrPtr < adr.Length && adr[adrPtr] == ',')
							{
								adrPtr++;
								pa2 = getpara(adr, ref adrPtr);
								gWork.wk.pan[1] = pa2;
							}
							break;
						case '\t':
							adrPtr++;
							break;
						case 'k':
							adrPtr++;
							kst = getpara(adr, ref adrPtr);
							gWork.wk.kst = kst;
							break;
						case '{':
							adrPtr++;
							ptmf = 1;
							break;
						case '*':
							adrPtr++;
							mac = getpara(adr, ref adrPtr);
							if (isnote(mac) != 0)
							{
								nt = DEF.DUMMY;
								endf = 1;
								break;
							}
							else//noteが含まれている
							{ break; }//			含まれていない
						case ';':
							while (adrPtr < adr.Length && adr[adrPtr] != '\n')
							{
								adrPtr++;
							}
							break;
						case '}':
							if (adrPtr == adr.Length - 1)
							{
								adrPtr++;
							}
							break;
						default:
							DEF.PF(string.Format("\nERROR!! ->{0} ! command not found", adr[adrPtr]));
							throw new M98Exception();
					}
				}

				if (endf != 0)
				{
					if (ptmf != 0)
					{
						while (adrPtr < adr.Length && adr[adrPtr] == '>')
						{
							adrPtr++;
							gWork.wk.oct++;
						}
						while (adrPtr < adr.Length && adr[adrPtr] == '<')
						{
							adrPtr++;
							gWork.wk.oct--;
						}
						ptm = nao2key(chr2nte(adr, adrPtr), gWork.wk.oct);
						while (adrPtr < adr.Length && adr[adrPtr] != '}')
						{
							adrPtr++;
						}
						adrPtr++;
						ptmf = 0;
					}
					if (adrPtr < adr.Length && adr[adrPtr] == '&')
					{
						adrPtr++;
						tief = 1;
					}
				}
			}

			//KUMA:プリザーブ向けTone情報更新
			if (gWork.preserveTone == null) gWork.preserveTone = new TONE(flag.OFF, this);
			gWork.preserveTone.puttone(nt, qt, ln, ot, vl, vi, tief, ptm, mac, pa1, pa2, kst, clk, ex);

			//KUMA:音符を見つけられなかった
			if (endf == 0)
			{
				return -1;
			}

			gWork.orgt[tnm].puttone(nt, qt, ln, ot, vl, vi, tief, ptm, mac, pa1, pa2, kst, clk, ex);
			return (adrPtr);       //次の音符のポインタを返す	
		}

		//---------------------------------------------------------------------------
		//ノートMMLのチェック
		//戻り値：キーナンバ（エラーの場合はNULL）
		private int getkey(string adr, ref int adrPtr, ref int now_nte, ref int now_oct)
		{
			try
			{
				switch (adr[adrPtr])
				{
					case 'c':
						now_nte = 0;
						adrPtr++;
						if (adrPtr < adr.Length && adr[adrPtr] == '+')
						{
							adrPtr++;
							now_nte++;
						}
						else if (adrPtr < adr.Length && adr[adrPtr] == '-')
						{
							adrPtr++;
							now_nte = 11;
							now_oct--;
						}
						break;
					case 'd':
						now_nte = 2;
						adrPtr++;
						if (adrPtr < adr.Length && adr[adrPtr] == '+')
						{
							adrPtr++;
							now_nte++;
						}
						else
						if (adrPtr < adr.Length && adr[adrPtr] == '-')
						{
							adrPtr++;
							now_nte--;
						}
						break;
					case 'e':
						now_nte = 4;
						adrPtr++;
						if (adrPtr < adr.Length && adr[adrPtr] == '+')
						{
							adrPtr++;
							now_nte++;
						}
						else
						if (adrPtr < adr.Length && adr[adrPtr] == '-')
						{
							adrPtr++;
							now_nte--;
						}
						break;
					case 'f':
						now_nte = 5;
						adrPtr++;
						if (adrPtr < adr.Length && adr[adrPtr] == '+')
						{
							adrPtr++;
							now_nte++;
						}
						else
						if (adrPtr < adr.Length && adr[adrPtr] == '-')
						{
							adrPtr++;
							now_nte--;
						}
						break;
					case 'g':
						now_nte = 7;
						adrPtr++;
						if (adrPtr < adr.Length && adr[adrPtr] == '+')
						{
							adrPtr++;
							now_nte++;
						}
						else
						if (adrPtr < adr.Length && adr[adrPtr] == '-')
						{
							adrPtr++;
							now_nte--;
						}
						break;
					case 'a':
						now_nte = 9;
						adrPtr++;
						if (adrPtr < adr.Length && adr[adrPtr] == '+')
						{
							adrPtr++;
							now_nte++;
						}
						else
						if (adrPtr < adr.Length && adr[adrPtr] == '-')
						{
							adrPtr++;
							now_nte--;
						}
						break;
					case 'b':
						now_nte = 11;
						adrPtr++;
						if (adrPtr < adr.Length && adr[adrPtr] == '+')
						{
							adrPtr++;
							now_nte++;
						}
						else
						if (adrPtr < adr.Length && adr[adrPtr] == '-')
						{
							adrPtr++;
							now_nte--;
						}
						break;
					default:
						return DEF.NotNote;
				}

				return nao2key(now_nte, now_oct);
			}
			catch
			{
				return DEF.NotNote;
			}
		}

		//---------------------------------------------------------------------------
		private int chr2nte(string a, int aPtr)
		{
			int i = 0;
			string c = "";
			while (a[aPtr] != '}')
			{
				c += a[aPtr];
				i++;
				aPtr++;
			}
			//c[i] = '\0';
			i = 0;
			while (c != gWork.notedat[i] && i < 12)
			{
				i++;
			}
			return i;
		}

		private int getlen(int x)
		{
			int cl = gWork.wk.clk / x;
			return (cl);
		}

		//---------------------------------------------------------------------------
		//文字列にcからbが含まれていれば0以外を返す
		private int isnote(int mac) //マクロの内容をサーチ
		{
			string a = gWork.mml[Common.searchm(mac)].mmldat;
			int aPtr = 0;
			for (; aPtr < a.Length && a[aPtr] != '\n'; aPtr++)
			{
				if (a[aPtr] == ';') return (DEF.EMPTY);
				int n = isnote(a, aPtr);
				if (n != 0) return n;
			}
			return 0;         //含まれていない
		}

		private int isnote(string a, int aPtr) //一文字をサーチ（多重定義）
		{
			if (a[aPtr] == 'c'
				|| a[aPtr] == 'd'
				|| a[aPtr] == 'e'
				|| a[aPtr] == 'f'
				|| a[aPtr] == 'g'
				|| a[aPtr] == 'a'
				|| a[aPtr] == 'b')
			{
				return (a[aPtr]); //含まれている
			}

			return 0;    //含まれていない
		}

		private void getcom(string a, ref int aPtr)
		{
			for (int i = 0; i < DEF.MAXCOMP; i++)
			{
				gWork.com.prm[0][i] = 0;//コマンドパラメータの初期化
				gWork.com.prm[1][i] = 0;
			}

			//初期化
			gWork.com.name = "";
			gWork.com.pnum = 0;
			gWork.com.spnum = 0;

			//コマンド名の取得
			char ch = a[aPtr];
			while (ch != ',' && ch != '(' && ch != '<' && ch != '-' && ch != '\0')
			{
				gWork.com.name += ch;
				ch = a[++aPtr];
			}

			//数値タイプのパラメータ取得
			int index = 0;
			while (aPtr < a.Length && a[aPtr] == '(')
			{
				if (gWork.com.name == "lgrp")
				{
					gWork.com.spnum = analyzeStringParam(a, ref aPtr, ')');
				}
				else
					gWork.com.pnum = analyzeNumericParam(index, a, ref aPtr);

				aPtr = SkipSpaceAndTab(a, aPtr);
				index++;
			}

			aPtr = SkipSpaceAndTab(a, aPtr);

			//文字タイプのパラメータ取得
			if (aPtr < a.Length && a[aPtr] == '<') gWork.com.spnum = analyzeStringParam(a, ref aPtr);

			aPtr = SkipSpaceAndTab(a, aPtr);

			//switchの対象を取得
			if (aPtr < a.Length && a[aPtr] == '-') analyzeSwitch(a, ref aPtr);
			if (gWork.atr.allflgck() == 0) gWork.atr.allflgon(); //すべてのフラグがOFFなら、すべてONにする

			aPtr = SkipSpaceAndTab(a, aPtr);

			if (aPtr >= a.Length || a[aPtr] != ',')
			{
				DEF.PF("コマンドの後の(,)がぬけています!");
				throw new M98Exception();
			}

			aPtr++;
		}

		private int SkipSpaceAndTab(string a, int aPtr)
		{
			while (aPtr < a.Length && (a[aPtr] == ' ' || a[aPtr] == '\t')) aPtr++;
			return aPtr;
		}

		private int analyzeNumericParam(int index,string a, ref int aPtr)
		{
			int y = 0;
			char ch = a[++aPtr];
			while (ch != ')' && y < DEF.MAXCOMP)
			{
				int x = getpara(a, ref aPtr);
				if (x == DEF.ERROR)
				{
					DEF.PF(string.Format("\nパラメーターエラー ->{0}", a));
					throw new M98Exception();
				}

				gWork.com.prm[index][y] = x; //set digit parameter
				y++;

				ch = a[aPtr];
				while (ch != ',' && ch != ')') ch = a[++aPtr];
				if (ch == ',') ch = a[++aPtr];
			}

			aPtr++;//skip ')'
			return y;
		}

		private int analyzeStringParam(string a, ref int aPtr, char em = '>')
		{
			int z = 0;
			aPtr++;
			if (aPtr == a.Length) return z;

			char ch = a[aPtr];

			while (ch != em && z < DEF.MAXCOMP)
			{
				gWork.com.sptr[z] = "";
				do
				{
					gWork.com.sptr[z] += ch.ToString();
					aPtr++;
					if (aPtr == a.Length) break;
					ch = a[aPtr];
				} while (ch != ',' && ch != em);
				z++;

				if (ch == ',') aPtr++;
				if (aPtr == a.Length) break;
				ch = a[aPtr];
			}
			aPtr++;//skip '>'
			return z;
		}

		private void analyzeSwitch(string a, ref int aPtr)
		{
			aPtr++;
			while (aPtr < a.Length && a[aPtr] != ' ' && a[aPtr] != ',')//optionchk(a[aPtr]) != 0)
			{
				char c = a[aPtr++];
				switch (c)
				{
					case 'n'://ノート
						gWork.atr.fnte = flag.ON;
						break;
					case 'l'://レングス
						gWork.atr.flen = flag.ON;
						break;
					case 'v'://ボリューム
						gWork.atr.fvol = flag.ON;
						break;
					case 'q'://クオンタイズ
						gWork.atr.fqtz = flag.ON;
						break;
					case '@'://音色
						gWork.atr.fvoi = flag.ON;
						break;
					case 'm'://マクロ
						gWork.atr.fmac = flag.ON;
						break;
					case '{'://ポルタメント
						gWork.atr.fptm = flag.ON;
						break;
					case 'p'://パン
						gWork.atr.fpan = flag.ON;
						break;
					case 'k'://キーシフト
						gWork.atr.fkst = flag.ON;
						break;
					//case 'C'://クロック
					//	gWork.atr.fclk = flag.ON;
					//	break;
					case 'e'://連符
						gWork.atr.fext = flag.ON;
						break;
					default:
						aPtr--;
						DEF.PF(string.Format(" -(parameter) Error! ->{0}", c));
						throw new M98Exception();
				}
			}

		}

		//-----------------------------------------------------------------
		//変換コマンドのチェック

		private char optionchk(char a)
		{
			if (a == 'n' || a == 'l' || a == 'v' || a == 'q' || a == '@' || a == 'm' ||
				a == '{' || a == 'p' || a == 'k' || a == 'C' || a == 'e')
				return (a);
			else
				return '\0';
		}

		//-----------------------------------------------------------------
		//cpytからあらたなsndbuf2を作る
		private void rmk_tone(int mml_num)
		{

			//#define comprt(c)	strcat(sndbuf2,(c))
			//#define putvalue(buffer)	strcat(sndbuf2,itoa((buffer),tmp,10))
			//#define noteput(nt)	strcat(sndbuf2,notedat[(nt)][0])	

			gWork.wk.nte = gWork.cpyt[0].getnte();
			gWork.wk.oct = gWork.cpyt[0].getoct();
			gWork.wk.qtz = gWork.cpyt[0].getqtz();
			gWork.wk.len = gWork.cpyt[0].getlen();
			gWork.wk.vol = gWork.cpyt[0].getvol();
			gWork.wk.voi = gWork.cpyt[0].getvoi();
			gWork.wk.kst = gWork.cpyt[0].getkst();
			gWork.wk.clk = gWork.cpyt[0].getclk();
			gWork.wk.pan[0] = gWork.cpyt[0].getpan(0);
			gWork.wk.pan[1] = gWork.cpyt[0].getpan(1);

			string tmp = "";
			gWork.sndbuf2 = " ";

			if (gWork.hismac != 0) //もしマクロ行なら
			{
				gWork.sndbuf2 += string.Format("*{0}{{ ", gWork.hismac - 1);
			}

			//ＭＭＬの第0要素の書き出し

			gWork.sndbuf2 += string.Format("C{0}", gWork.wk.clk);
			getlchr(gWork.wk.len, ref tmp);
			gWork.sndbuf2 += tmp;
			gWork.sndbuf2 += string.Format("o{0}", gWork.wk.oct);

			if (gWork.wk.vol != DEF.EMPTY) gWork.sndbuf2 += string.Format("v{0}", gWork.wk.vol);
			if (gWork.wk.qtz != DEF.EMPTY) gWork.sndbuf2 += string.Format("q{0}", gWork.wk.qtz);
			if (gWork.wk.voi != DEF.EMPTY) gWork.sndbuf2 += string.Format("@{0}", gWork.wk.voi);
			if (gWork.wk.pan[0] != DEF.EMPTY || ((gWork.wk.pan[0] > 3 && gWork.wk.pan[0] < 7) && gWork.wk.pan[1] != DEF.EMPTY))
			{
				gWork.sndbuf2 += string.Format("p{0}", gWork.wk.pan[0]);
				if((gWork.wk.pan[0] > 3 && gWork.wk.pan[0] < 7) && gWork.wk.pan[1] != DEF.EMPTY)
				{
					gWork.sndbuf2 += string.Format(",{0}", gWork.wk.pan[1]);
				}
			}
			if (gWork.wk.kst != DEF.EMPTY) gWork.sndbuf2 += string.Format("k{0}", gWork.wk.kst);
			if (gWork.initf_vol == flag.ON && gWork.wk.vol == DEF.EMPTY) gWork.sndbuf2 += "v12";
			if (gWork.initf_qtz == flag.ON && gWork.wk.qtz == DEF.EMPTY) gWork.sndbuf2 += "q0";
			if (gWork.initf_voi == flag.ON && gWork.wk.voi == DEF.EMPTY) gWork.sndbuf2 += string.Format("@{0}", gWork.random.Next(256));
			if (gWork.initf_pan == flag.ON && gWork.wk.pan[0] == DEF.EMPTY) gWork.sndbuf2 += "p3";
			if (gWork.initf_kst == flag.ON && gWork.wk.kst == DEF.EMPTY) gWork.sndbuf2 += "k0";
			if (gWork.cpyt[0].getmac() != 0) gWork.sndbuf2 += string.Format("*{0}", gWork.cpyt[0].getmac());

			prtnte(gWork.cpyt[0]);


			//ＭＭＬの第i要素の書き出し

			for (int i = 1; i <= mml_num; i++)
			{
				prtclk(gWork.cpyt[i]);
				prtqtz(gWork.cpyt[i]);
				prtoct(gWork.cpyt[i]);
				prtvol(gWork.cpyt[i]);
				prtvoi(gWork.cpyt[i]);
				prtpan(gWork.cpyt[i]);
				prtkst(gWork.cpyt[i]);
				prtmac(gWork.cpyt[i]);
				prtnte(gWork.cpyt[i]);
			}


			//プリザーブ処理
			prtPreserve();

			gWork.sndbuf2 += (gWork.hismac != 0 ? " }\r\n" : "\r\n");
		}

		//プリザーブの書き出し
		private void prtPreserve()
		{
			if (!gWork.preserve) return;

			if (gWork.preserveFlags.fclk == flag.ON) prtclk(gWork.preserveTone);
			if (gWork.preserveFlags.flen == flag.ON || gWork.preserveFlags.fnte == flag.ON)
			{
				prtlen(gWork.preserveTone);
				prtoct(gWork.preserveTone);
			}
			if (gWork.preserveFlags.fvoi == flag.ON) prtvoi(gWork.preserveTone);
			if (gWork.preserveFlags.fvol == flag.ON) prtvol(gWork.preserveTone);
			if (gWork.preserveFlags.fkst == flag.ON) prtkst(gWork.preserveTone);
			if (gWork.preserveFlags.fpan == flag.ON) prtpan(gWork.preserveTone);
			if (gWork.preserveFlags.fqtz == flag.ON) prtqtz(gWork.preserveTone);
		}

		//-----------------------------------------------------------------
		//ＭＭＬの書き出し関数群

		private void prtnte(TONE tn)
		{
			//	ポルタメントのとき

			if (tn.getptm() != 0 && tn.getnte() != DEF.DUMMY && tn.getnte() != DEF.REST)
			{
				prtlen(tn);
				prtptm(tn);
				prttie(tn);
			}
			else

			//	通常のノートのとき

			if (tn.getnte() != DEF.DUMMY && tn.getnte() != DEF.REST)
			{
				prtlen(tn);
				noteput(tn.getnte());
				prttie(tn);

			}
			else

			//	休符のとき

			if (tn.getnte() == DEF.REST)
			{
				prtlen(tn);
				comprt("r");
			}

			//	連符チェック

			int exn = tn.getext();     //連符数
			if (exn > 1)
			{   //もし連符なら

				TONE p, q;
				p = tn.extadr;
				for (exn--; exn != 0; exn--)
				{
					if (p.getqtz() != DEF.EMPTY) prtqtz(p);
					prtoct(p);
					if (p.getvol() != DEF.EMPTY) prtvol(p);
					if (p.getvoi() != DEF.EMPTY) prtvoi(p);
					if (p.getpan(0) != DEF.EMPTY || ((p.getpan(0) > 3 && p.getpan(0) < 7) && p.getpan(1) != DEF.EMPTY))
						prtpan(p);
					if (p.getkst() != DEF.EMPTY) prtkst(p);
					prtmac(p);
					prtlen(p);
					noteput(p.getnte());
					prttie(p);
					q = p.extadr;
					//delete p;
					p = q;
				}
				//delete p;
				//printf("after:%lu\n",coreleft());
			}
		}

		private void comprt(string c)
		{
			gWork.sndbuf2 += c;
		}

		private void putvalue(int buffer)
		{
			gWork.sndbuf2 += buffer;
		}

		private void noteput(int nt)
		{
			gWork.sndbuf2 += gWork.notedat[(nt)];//[0];
		}

		private void prtptm(TONE p)
		{
			if (p.getptm() == 0) return;
			int n, o = 0;
			comprt("{");
			noteput(p.getnte());
			key2nao(p.getptm(), out n, out o);
			while (p.getoct() < o) { comprt(">"); o--; gWork.wk.oct++; }
			while (p.getoct() > o) { comprt("<"); o++; gWork.wk.oct--; }
			noteput(n);
			comprt("}");
		}

		private void prtqtz(TONE p)
		{
			int n = p.getqtz();
			if (n == DEF.EMPTY) return;

			//char tmp[10];
			if (n != gWork.wk.qtz)
			{
				comprt("q");
				putvalue(n);
				gWork.wk.qtz = n;
			}
		}

		private void prtoct(TONE p)
		{
			int n = p.getoct();
			if (n == DEF.EMPTY) return;

			while (n < gWork.wk.oct)
			{
				comprt("<");
				gWork.wk.oct--;
			}
			while (n > gWork.wk.oct)
			{
				comprt(">");
				gWork.wk.oct++;
			}
		}

		private void prtvol(TONE p)
		{
			int n = p.getvol();
			if (n == DEF.EMPTY) return;

			if (n != gWork.wk.vol)
			{
				comprt("v");
				putvalue(n);
				gWork.wk.vol = n;
			}
		}

		private void prtvoi(TONE p)
		{
			int n = p.getvoi();
			if (n == DEF.EMPTY) return;

			if (n != gWork.wk.voi)
			{
				comprt("@");
				putvalue(n);
				gWork.wk.voi = n;
			}
		}

		private void prtpan(TONE p)
		{
			int n1 = p.getpan(0);
			int n2 = p.getpan(1);
			if (n1 == DEF.EMPTY || ((n1 > 3 && n1 < 7) && n2 == DEF.EMPTY)) return;

			if (n1 > 0 && n1 < 4)
			{
				if (n1 != gWork.wk.pan[0])
				{
					comprt("p");
					putvalue(n1);
				}

				gWork.wk.pan[0] = n1;
				gWork.wk.pan[1] = DEF.EMPTY;
			}
			else
			{
				if (n1 != gWork.wk.pan[0] || n2 != gWork.wk.pan[1])
				{
					gWork.sndbuf2 += string.Format("p{0},{1}", n1, n2);
				}

				gWork.wk.pan[0] = n1;
				gWork.wk.pan[1] = n2;
			}
		}

		private void prtclk(TONE p)
		{
			int n = p.getclk();
			if (n == DEF.EMPTY) return;

			if (n != gWork.wk.clk)
			{
				comprt("C");
				putvalue(n);
				gWork.wk.clk = p.getclk();
			}
		}

		private void prtkst(TONE p)
		{
			int n = p.getkst();
			if (n == DEF.EMPTY) return;

			if (n != gWork.wk.kst)
			{
				comprt("k");
				putvalue(n);
				gWork.wk.kst = n;
			}
		}

		private void prtmac(TONE p)
		{
			int n = p.getmac();
			if (n == DEF.EMPTY) return;

			if (n != 0)
			{
				comprt("*");
				putvalue(n);
			}
		}

		private void prtlen(TONE p)
		{
			int n = p.getlen();
			if (n == DEF.EMPTY) return;

			if (n != gWork.wk.len)
			{
				string tmp = "";
				getlchr(n, ref tmp);
				gWork.sndbuf2 += tmp;
				gWork.wk.len = n;
			}
		}

		private void prttie(TONE p)
		{
			int n = p.gettie();
			if (n == DEF.EMPTY) return;

			if (n != 0)
			{
				comprt("&");
			}
		}

		//-----------------------------------------------------------------
		//乱数フィルタセット
		private void setrfil()
		{
			gWork.randmin = gWork.com.prm[0][0];
			gWork.randmax = gWork.com.prm[0][1];
			if (gWork.randmin > gWork.randmax)
			{
				int n = gWork.randmin;
				gWork.randmin = gWork.randmax;
				gWork.randmax = n;
			}
		}

		//-----------------------------------------------------------------
		//リピートセット
		private void setrep()
		{
			gWork.rp.min = gWork.com.prm[0][0];
			gWork.rp.max = gWork.com.prm[0][1];
			if (gWork.rp.min > gWork.rp.max)
			{
				int n = gWork.rp.min;
				gWork.rp.min = gWork.rp.max;
				gWork.rp.max = n;
			}
			if (gWork.atr.fnte == flag.ON) { gWork.rp.f_nte = flag.ON; }
			if (gWork.atr.fvol == flag.ON) { gWork.rp.f_vol = flag.ON; }
			if (gWork.atr.flen == flag.ON) { gWork.rp.f_len = flag.ON; }
			if (gWork.atr.fqtz == flag.ON) { gWork.rp.f_qtz = flag.ON; }
			if (gWork.atr.fvoi == flag.ON) { gWork.rp.f_voi = flag.ON; }
			if (gWork.atr.fkst == flag.ON) { gWork.rp.f_kst = flag.ON; }
			gWork.rp.s_rep(); //couter initiarize
		}

		//-----------------------------------------------------------------
		//＠グループセット
		private void s_grpvoi()
		{
			int i = 0;
			int lp = gWork.com.pnum;
			while (lp != 0 && i < DEF.MAXVOIGR)
			{
				gWork.grp.voi[i] = gWork.com.prm[0][i];
				i++; lp--;
			}
			gWork.voigrf = i;
		}

		//-----------------------------------------------------------------
		//pグループセット
		private void s_grpPan()
		{
			int i = 0;
			int lp = gWork.com.pnum;
			while (lp != 0 && i < DEF.MAXVOIGR)
			{
				gWork.grp.pan[0][i] = gWork.com.prm[0][i];
				gWork.grp.pan[1][i] = gWork.com.prm[1][i];
				i++; lp--;
			}

			gWork.pangrf = i;

			//swap

			if (gWork.grp.pan[0][0] > gWork.grp.pan[0][1])
			{
				i = gWork.grp.pan[0][0];
				gWork.grp.pan[0][0]= gWork.grp.pan[0][1];
				gWork.grp.pan[0][1] = i;
			}

			if (gWork.grp.pan[1][0] > gWork.grp.pan[1][1])
			{
				i = gWork.grp.pan[1][0];
				gWork.grp.pan[1][0] = gWork.grp.pan[1][1];
				gWork.grp.pan[1][1] = i;
			}

		}

		//-----------------------------------------------------------------
		//ｌグループセット
		//private void s_grplen()
		//{
		//	int i = 0;
		//	int lp = gWork.com.pnum;
		//	while (lp != 0 && i < DEF.MAXLENGR)
		//	{
		//		gWork.grp.len[i] = gWork.com.prm[i];
		//		//gWork.grp.len[i] = getlen(gWork.grp.len[i]);
		//		i++;
		//		lp--;
		//	}
		//	gWork.lengrf = i;
		//}
		private void s_grplen()
		{
			int i = 0;
			int lp = gWork.com.spnum;
			while (lp != 0 && i < DEF.MAXLENGR)
			{
				string s = gWork.com.sptr[i];
				int ptr = 0;
				int ln = 0;
				int x = getpara(s, ref ptr);
				if (x == DEF.ERROR) throw new M98Exception();
				ln = getlen(x);
				if (ptr < s.Length && s[ptr] == '.')
				{
					ptr++;
					ln = ln + (ln / 2);
				}
				gWork.grp.len[i] = ln;

				i++;
				lp--;
			}
			gWork.lengrf = i;
		}

		//-----------------------------------------------------------------
		//マクログループセット
		private void s_grpmac()
		{
			int lp, i;
			for (lp = gWork.com.pnum, i = 0; (lp != 0) && (i < DEF.MAXMACGR); i++, lp--)
			{
				gWork.grp.mac[i] = gWork.com.prm[0][i];
			}
			gWork.macgrf = i;
		}
		//-----------------------------------------------------------------
		//ノートグループセット
		private void s_grpnte()
		{
			//#define cps(x)	strcmp(stp[i],(x))

			int nt = 0, ot = 0;
			int i, gpnum = 0;

			STRPRM stp = new STRPRM(); //ストリングパラメータ変数
			int p;

			for (i = 0; i < gWork.com.spnum; i++)
			{
				if (stp.ope_kakko(i) == "all1"
					|| stp.ope_kakko(i) == "all1"
					|| stp.ope_kakko(i) == "all2"
					|| stp.ope_kakko(i) == "all3"
					|| stp.ope_kakko(i) == "all4"
					|| stp.ope_kakko(i) == "all5"
					|| stp.ope_kakko(i) == "all6"
					|| stp.ope_kakko(i) == "all7"
					|| stp.ope_kakko(i) == "all8"
					)
				{
					p = 3;// stp.ope_kakko(i)[3];
					if (Common.isdigit(stp.ope_kakko(i), p))
					{
						ot = int.Parse(stp.ope_kakko(i).Substring(p));//get octave
					}
					else
					{
						DEF.PF("\nall?...オクターブ指定");
						prmerr();
						throw new M98Exception();
					}
					for (nt = 0; nt <= 11; nt++, gpnum++)
					{
						gWork.grp.setnte(gpnum, nao2key(nt, ot));//set key
					}
				}
				else
				{
					p = 0;// stp[i]; //第iパラメータ
					if (!Common.isdigit(stp.ope_kakko(i), p))
					{
						DEF.PF("\nノートグループの数値");
						prmerr();
						throw new M98Exception();
					}
					ot = Common.atoi(stp.ope_kakko(i), p);//オクターブ取得
					p++;
					if (isnote(stp.ope_kakko(i), p) == 0)
					{
						DEF.PF("\nノートグループのノート");
						prmerr();
						throw new M98Exception();
					}
					gWork.grp.setnte(gpnum, getkey(stp.ope_kakko(i), ref p, ref nt, ref ot));    //set key
					gpnum++;
				}
			}
			gWork.ntegrf = gpnum;

			//#undef cps
		}

		//----------------------------------------------------------------
		//パラメータエラー表示
		private void prmerr()
		{
			DEF.PF("パラメータエラーです");
			//beep();
		}

		//-----------------------------------------------------------------
		//クオンタイズグループセット
		private void s_grpqtz()
		{
			int lp = gWork.com.pnum;
			int i;
			for (i = 0; (lp != 0) && (i < DEF.MAXQTZGR); i++)
			{
				gWork.grp.qtz[i] = gWork.com.prm[0][i];
				lp--;
			}
			gWork.qtzgrf = i;
		}

		//-----------------------------------------------------------------
		//キーシフトグループセット
		private void s_grpkst()
		{
			int lp = gWork.com.pnum;
			int i;
			for (i = 0; (lp != 0) && (i < DEF.MAXKSTGR); i++, lp--)
			{
				gWork.grp.kst[i] = gWork.com.prm[0][i];
			}
			gWork.kstgrf = i;
		}

		//-----------------------------------------------------------------
		//連符グループセット
		private void s_grpext()
		{
			int lp = gWork.com.pnum;
			int i;
			for (i = 0; (lp != 0) && (i < DEF.MAXEXTGR); i++, lp--)
			{
				gWork.grp.ext[i] = gWork.com.prm[0][i];
			}
			gWork.extgrf = i;
		}
		//-----------------------------------------------------------------
		//＠グループlineセット
		private void s_voigrl()
		{
			int i = gWork.com.prm[0][0];
			if (gWork.com.prm[0][0] > gWork.com.prm[0][1])
			{
				gWork.com.prm[0][0] = gWork.com.prm[0][1];
				gWork.com.prm[0][1] = i;
			}
			int j = 0;
			for (; i <= gWork.com.prm[0][1]; i++)
			{
				gWork.grp.voi[j] = i;
				j++;
				if (j > DEF.MAXVOIGR)
				{
					DEF.PF("音色グループオーバー\n");
					throw new M98Exception();
				}
			}
			gWork.voigrf = j;
		}

		//-----------------------------------------------------------------
		//マクログループlineセット
		private void s_macgrl()
		{
			int i = gWork.com.prm[0][0];
			if (gWork.com.prm[0][0] > gWork.com.prm[0][1])
			{
				gWork.com.prm[0][0] = gWork.com.prm[0][1];
				gWork.com.prm[0][1] = i;
			}
			int j = 0;
			for (; i <= gWork.com.prm[0][1]; i++)
			{
				gWork.grp.mac[j] = i;
				j++;
				if (j > DEF.MAXMACGR)
				{
					DEF.PF("マクログループオーバー\n");
					throw new M98Exception();
				}
			}
			gWork.macgrf = j;
		}

		//-----------------------------------------------------------------
		//ＭＭＬ初期化設定
		private void initmml()
		{
			if (gWork.atr.fqtz != flag.OFF) gWork.initf_qtz = flag.ON;
			if (gWork.atr.fvol != flag.OFF) gWork.initf_vol = flag.ON;
			if (gWork.atr.fvoi != flag.OFF) gWork.initf_voi = flag.ON;
			if (gWork.atr.fpan != flag.OFF) gWork.initf_pan = flag.ON;
			if (gWork.atr.fkst != flag.OFF) gWork.initf_kst = flag.ON;
		}

		//-----------------------------------------------------------------
		//変換マスク属性設定
		private void setmsk()
		{
			if (gWork.com.pnum == 0) return;
			int point = 0;// gWork.com.prm;       //マスクされるべきところ
			for (int i = 1; i <= gWork.com.pnum; i++)
			{
				gWork.atr.putall(gWork.com.prm[0][point++], atrb.MASK); //マスク属性
			}
		}
		//-----------------------------------------------------------------
		//変換マスク属性設定line
		private void setmskl()
		{
			if (gWork.com.pnum == 0) return;
			int p = 0;// com.prm;       //マスクされるべきところ
			int mskstart, mskend;
			for (int i = 1; i <= gWork.com.pnum / 2; i++)
			{
				mskstart = gWork.com.prm[0][p++];
				mskend = gWork.com.prm[0][p++];
				if (mskstart > mskend)
				{
					int tmp = mskstart;
					mskstart = mskend;
					mskend = tmp;
				}
				for (int y = mskstart; y <= mskend; y++)
				{
					gWork.atr.putall(y, atrb.MASK);    //マスク属性
				}
			}
		}

		//-----------------------------------------------------------------
		//セレクトされたところ以外はマスク
		private void setslct(int tonenum)
		{
			if (gWork.com.pnum == 0) return;
			int flag;
			for (int i = 0; i <= tonenum; i++)
			{
				flag = 0;
				int p = 0;// gWork.com.prm;
				for (int y = 1; y <= gWork.com.pnum; y++)
				{
					if (gWork.com.prm[0][p++] == i) flag = 1;
				}
				if (flag == 0) gWork.atr.putall(i, atrb.MASK);
			}
		}
		//-----------------------------------------------------------------
		//セレクトされた範囲以外はマスク
		private void setslctl(int tonenum)
		{
			if (gWork.com.pnum == 0) return;
			qsort(
				ref gWork.com.prm[0],
				gWork.com.pnum,
				//sizeof(int),       //com.prmを小から昇順に
				cmpi
				);    //クイックソートする	
			int flag;
			for (int i = 0; i <= tonenum; i++)
			{
				flag = 0;
				int p = 0;// gWork.com.prm;
				for (int y = 1; y <= gWork.com.pnum / 2; y++)
				{
					int sst = gWork.com.prm[0][p++];
					int send = gWork.com.prm[0][p++];
					if (sst <= i && send >= i) flag = 1;
				}
				if (flag == 0) gWork.atr.putall(i, atrb.MASK);
			}

		}

		private void qsort(ref int[] prm, int pnum, Func<int, int, int> cmpi)
		{
			for (int i = 0; i < pnum - 1; i++)
			{
				for (int j = i; j < pnum; j++)
				{
					if (cmpi(prm[i], prm[j]) > 0)
					{
						int n = prm[i];
						prm[i] = prm[j];
						prm[j] = prm[i];
					}
				}
			}
		}

		private int cmpi(int a, int b)    //整数比較関数
		{
			return a - b;
		}

		//------------------------------------------------------------------
		//ランダムアクセント	連符にランダムなアクセントをつける
		private void rndacc()
		{
			for (int i = 0; i <= gWork.TNUM; i++)
			{
				if (gWork.cpyt[i].getext() != 1)
				{
					TONE p;
					p = gWork.cpyt[i].extadr;
					for (int y = 1; y < gWork.cpyt[i].getext(); y++)
					{
						int acc = gWork.random.Next(gWork.com.prm[0][0]) + 1;
						int v = p.getvol() + acc;
						if (v > 15) p.putvol(15);
						else p.putvol(v);
						p = p.extadr;
					}
				}
			}

		}

		//------------------------------------------------------------------
		//連符に対するマスク
		private void setmskext()
		{


		}

		//------------------------------------------------------------------
		//各コマンドの許可／禁止
		private void comswitch(flag f)
		{
			if (f == flag.OFF) gWork.atr.disable_flag();   //各フラグ禁止
			else gWork.atr.enable_flag(); //各フラグ許可

			int lp = gWork.com.spnum;
			STRPRM stp = new STRPRM();
			for (int i = 0; lp != 0; i++)
			{
				gWork.com.on_off(stp.ope_kakko(i), f);
				lp--;
			}
		}

		//-----------------------------------------------------------------
		//新たな構造体cpytを作る		
		private void mk_fprg(int x)
		{

			int i, y;
			int prog = 0;// gWork.progress;   //数列バッファ

			for (i = 0; i <= x; i++)
			{
				y = gWork.progress[prog];

				//ノート
				if (gWork.atr.getnte(i) == atrb.DISABLE)
				{
					gWork.cpyt[i].putnte(gWork.orgt[i].getnte());
					gWork.cpyt[i].putoct(gWork.orgt[i].getoct());
					gWork.cpyt[i].puttie(gWork.orgt[i].gettie());
				}
				else if (gWork.atr.getnte(i) == atrb.CRAND)
				{
					gWork.cpyt[i].putnte(gWork.orgt[y].getnte());
					gWork.cpyt[i].putoct(gWork.orgt[y].getoct());
					gWork.cpyt[i].puttie(gWork.orgt[y].gettie());
				}
				else if (gWork.atr.getnte(i) == atrb.RAND)
				{
					int nt = 0, ot = 0;
					newrnte(ref nt, ref ot, i);
					gWork.cpyt[i].putnte(nt);
					gWork.cpyt[i].putoct(ot);
					if (gWork.orgt[i].getptm() > 0)//もしポルタメントだったら
					{
						gWork.cpyt[i].putptm(rndpkey(gWork.cpyt[i].getnte(), gWork.cpyt[i].getoct()));
					}
				}

				//ボリューム
				if (gWork.atr.getvol(i) == atrb.DISABLE)
				{
					gWork.cpyt[i].putvol(gWork.orgt[i].getvol());
				}
				else
				if (gWork.atr.getvol(i) == atrb.CRAND)
				{
					gWork.cpyt[i].putvol(gWork.orgt[y].getvol());
				}
				else
				if (gWork.atr.getvol(i) == atrb.RAND)
				{
					newrvol(i);
				}

				//レングス
				if (gWork.atr.getlen(i) == atrb.DISABLE)
				{
					gWork.cpyt[i].putlen(gWork.orgt[i].getlen());
				}
				else
				if (gWork.atr.getlen(i) == atrb.CRAND)
				{
					gWork.cpyt[i].putlen(gWork.orgt[y].getlen());
				}
				else
				if (gWork.atr.getlen(i) == atrb.RAND)
				{
					newrlen(i);
				}

				//クオンタイズ
				if (gWork.atr.getqtz(i) == atrb.DISABLE)
				{
					gWork.cpyt[i].putqtz(gWork.orgt[i].getqtz());
				}
				else
				if (gWork.atr.getqtz(i) == atrb.CRAND)
				{
					gWork.cpyt[i].putqtz(gWork.orgt[y].getqtz());
				}
				else
				if (gWork.atr.getqtz(i) == atrb.RAND)
				{
					newrqtz(i);
				}

				//音色
				if (gWork.atr.getvoi(i) == atrb.DISABLE)
				{
					gWork.cpyt[i].putvoi(gWork.orgt[i].getvoi());
				}
				else if (gWork.atr.getvoi(i) == atrb.CRAND)
				{
					gWork.cpyt[i].putvoi(gWork.orgt[y].getvoi());
				}
				else if (gWork.atr.getvoi(i) == atrb.RAND)
				{
					newrvoi(i);
				}

				//マクロ
				if (gWork.atr.getmac(i) == atrb.DISABLE)
				{
					gWork.cpyt[i].putmac(gWork.orgt[i].getmac());
				}
				else
				if (gWork.atr.getmac(i) == atrb.CRAND)
				{
					gWork.cpyt[i].putmac(gWork.orgt[y].getmac());
				}
				else
				if (gWork.atr.getmac(i) == atrb.RAND)
				{
					newrmac(i);
				}

				//連符		注：連符のCRANDは無い
				if (gWork.atr.getext(i) == atrb.DISABLE)
				{
					gWork.cpyt[i].putext(gWork.orgt[i].getext());
				}
				if (gWork.atr.getext(i) == atrb.RAND)
				{
					newrext(i);
				}

				//ポルタメント
				switch (gWork.atr.getptm(i))
				{
					case atrb.DISABLE:
						gWork.cpyt[i].putoct(gWork.orgt[i].getoct());
						gWork.cpyt[i].putnte(gWork.orgt[i].getnte());
						gWork.cpyt[i].putptm(gWork.orgt[i].getptm());
						break;
					case atrb.CRAND:
						gWork.cpyt[i].putoct(gWork.orgt[y].getoct());
						gWork.cpyt[i].putnte(gWork.orgt[y].getnte());
						gWork.cpyt[i].putptm(gWork.orgt[y].getptm());
						break;
					case atrb.RAND:
						newrptm(ref gWork.orgt[i], ref gWork.cpyt[i]);
						break;
				}

				//キーしふと
				switch (gWork.atr.getkst(i))
				{
					case atrb.DISABLE:
						gWork.cpyt[i].putkst(gWork.orgt[i].getkst());
						break;
					case atrb.CRAND:
						gWork.cpyt[i].putkst(gWork.orgt[y].getkst());
						break;
					case atrb.RAND:
						newrkst(i);
						break;
				}

				//パン
				switch (gWork.atr.getpan(i))
				{
					case atrb.DISABLE:
						gWork.cpyt[i].putpan(0, gWork.orgt[i].getpan(0));
						gWork.cpyt[i].putpan(1, gWork.orgt[i].getpan(1));
						break;
					case atrb.CRAND:
						gWork.cpyt[i].putpan(0, gWork.orgt[y].getpan(0));
						gWork.cpyt[i].putpan(1, gWork.orgt[y].getpan(1));
						break;
					case atrb.RAND:
						newrpan(i);
						break;
				}

				//クロック
				switch (gWork.atr.getclk(i))
				{
					case atrb.DISABLE:
						gWork.cpyt[i].putclk(gWork.orgt[i].getclk());
						break;
					case atrb.CRAND:
						gWork.cpyt[i].putclk(gWork.orgt[y].getclk());
						break;
					case atrb.RAND:
						newrclk(i);
						break;
				}

				prog++;
			}

		}

		private int rndpkey(int n, int o)
		{
			int orgk = nao2key(n, o);
			int chgk;

			while (true)
			{
				chgk = orgk + (gWork.random.Next(24) - 12); //randomは-12から
				if (chgk != orgk && chgk >= 0 && chgk <= 95) break;
			}

			return chgk;
		}

		//------------------------------------------------------------------
		//新しい乱数キーシフトを設定する
		private void newrkst(int i)
		{
			if (gWork.kstgrf != 0)
			{   
				//もしキーシフトグループが設定されてたら
				if (i != 0 && gWork.rp.c_kst != 0)
				{
					gWork.cpyt[i].putkst(gWork.cpyt[i - 1].getkst());
					gWork.rp.c_kst--;
					return;
				}

				gWork.cpyt[i].putkst(gWork.grp.kst[gWork.random.Next(gWork.kstgrf)]);
				gWork.rp.s_rep(); //couter initiarize
				return;

			}

			//それ以外
			if (i != 0 && gWork.rp.c_kst != 0)
			{
				gWork.cpyt[i].putkst(gWork.cpyt[i - 1].getkst());
				gWork.rp.c_kst--;
				return;
			}

			gWork.cpyt[i].putkst(rndfil(13));
			gWork.rp.s_rep(); //couter initiarize

		}

		//------------------------------------------------------------------
		//新しい乱数ボリュームを設定する
		private void newrvol(int i)
		{
			if (i != 0 && gWork.rp.c_vol != 0)
			{
				gWork.cpyt[i].putvol(gWork.cpyt[i - 1].getvol());
				gWork.rp.c_vol--;
				return;
			}

			gWork.cpyt[i].putvol(rndfil(16));
			gWork.rp.s_rep(); //couter initiarize

		}

		//------------------------------------------------------------------
		//新しい乱数レングスを設定する
		private void newrlen(int i)
		{
			if (gWork.lengrf != 0)
			{       //もしレングスグループが設定されてたら
				if (i != 0 && gWork.rp.c_len != 0)
				{
					gWork.cpyt[i].putlen(gWork.cpyt[i - 1].getlen());
					gWork.rp.c_len--;
					return;
				}

				gWork.cpyt[i].putlen(gWork.grp.len[gWork.random.Next(gWork.lengrf)]);
				gWork.rp.s_rep(); //couter initiarize
				return;
			}

			//それ以外
			if (i != 0 && gWork.rp.c_len != 0)
			{
				gWork.cpyt[i].putlen(gWork.cpyt[i - 1].getlen());
				gWork.rp.c_len--;
				return;
			}

			//cpyt[i].length=wk.msl*(rndfil(wk.clk/wk.msl)+1);
			gWork.cpyt[i].putlen(rndfil(gWork.wk.clk) + 1);
			gWork.rp.s_rep(); //couter initiarize

		}

		//------------------------------------------------------------------
		//新しい乱数クオンタイズを設定する
		private void newrqtz(int i)
		{
			if (gWork.qtzgrf != 0)
			{       //もしクオンタイズグループが設定されてたら
				if (i != 0 && gWork.rp.c_qtz != 0)
				{
					gWork.cpyt[i].putqtz(gWork.cpyt[i - 1].getqtz());
					gWork.rp.c_qtz--;
					return;
				}

				gWork.cpyt[i].putqtz(gWork.grp.qtz[gWork.random.Next(gWork.qtzgrf)]);
				gWork.rp.s_rep(); //couter initiarize
				return;
			}

			if (i != 0 && gWork.rp.c_qtz != 0)
			{
				gWork.cpyt[i].putqtz(gWork.cpyt[i - 1].getqtz());
				gWork.rp.c_qtz--;
				return;
			}

			gWork.cpyt[i].putqtz(rndfil(gWork.cpyt[i].getlen()));
			gWork.rp.s_rep(); //couter initiarize

		}

		//------------------------------------------------------------------
		//新しい乱数音色を設定する
		private void newrvoi(int i)
		{
			if (gWork.voigrf != 0)
			{       //もし音色グループが設定されてたら
				if (i != 0 && gWork.rp.c_voi != 0)
				{
					gWork.cpyt[i].putvoi(gWork.cpyt[i - 1].getvoi());
					gWork.rp.c_voi--;
					return;
				}

				gWork.cpyt[i].putvoi(gWork.grp.voi[gWork.random.Next(gWork.voigrf)]);
				gWork.rp.s_rep(); //couter initiarize
				return;
			}

			//それ以外
			if (i != 0 && gWork.rp.c_voi != 0)
			{
				gWork.cpyt[i].putvoi(gWork.cpyt[i - 1].getvoi());
				gWork.rp.c_voi--;
				return;
			}

			gWork.cpyt[i].putvoi(rndfil(256));
			gWork.rp.s_rep(); //couter initiarize

		}

		//------------------------------------------------------------------
		//新しい乱数マクロを設定する
		private void newrmac(int i)
		{
			if (gWork.macgrf != 0)
			{       //もしマクログループが設定されてたら
				gWork.cpyt[i].putmac(gWork.grp.mac[gWork.random.Next(gWork.macgrf)]);
				return;
			}

			gWork.cpyt[i].putmac(0);
			//PF("\nWarning...乱数によるマクロ指定はグループを設定して下さい\n");
			//ERREND;

		}

		//------------------------------------------------------------------
		//新しい乱数連符を設定する
		private void newrext(int i)
		{
			if (gWork.extgrf != 0)
			{
				//もし連符グループが設定されてたら
				if (i != 0 && gWork.rp.c_ext != 0)
				{   //連符リピートあり
					gWork.cpyt[i].putext(gWork.cpyt[i - 1].getext());
					gWork.cpyt[i].mk_ext(i);
					gWork.rp.c_ext--;
					return;
				}

				//連符リピートなし
				int ex = gWork.grp.ext[gWork.random.Next(gWork.extgrf)];   //連符の数
				gWork.cpyt[i].putext(ex);
				gWork.cpyt[i].mk_ext(i);
				gWork.rp.s_rep(); //couter initiarize
				return;
			}

			if (i != 0 && gWork.rp.c_ext != 0)
			{
				gWork.cpyt[i].putext(gWork.cpyt[i - 1].getext());
				gWork.cpyt[i].mk_ext(i);
				gWork.rp.c_ext--;
				return;
			}

			gWork.cpyt[i].putext(rndfil(4) + 1);
			gWork.cpyt[i].mk_ext(i);
			gWork.rp.s_rep();

		}

		//------------------------------------------------------------------
		//新しい乱数ポルタメント設定する
		private void newrptm(ref TONE org, ref TONE cp)
		{
			if (rndfil(100) > 50)
			{
				cp.putptm(rndpkey(org.getnte(), org.getoct()));
				return;
			}

			cp.putptm(0);   //ﾎﾟﾙﾀﾒﾝﾄ消去

		}

		//------------------------------------------------------------------
		//新しい乱数パンを設定する
		private void newrpan(int i)
		{
			int pan1;
			int pan2;
			int clk = 255;// Math.Max(Math.Min(gWork.wk.clk, 255), 1);

			if (i != 0 && gWork.rp.c_pan != 0)
			{
				pan1 = gWork.cpyt[i - 1].getpan(0);
				pan2 = gWork.cpyt[i - 1].getpan(1);
				gWork.cpyt[i].putpan(0, pan1);
				gWork.cpyt[i].putpan(1, pan2);
				gWork.rp.c_pan--;
				return;
			}


			if (gWork.pangrf != 0)
			{
				pan1 = gWork.random.Next(gWork.grp.pan[0][1] - gWork.grp.pan[0][0]) + gWork.grp.pan[0][0];
				pan2 = gWork.random.Next(gWork.grp.pan[1][1] - gWork.grp.pan[1][0]) + gWork.grp.pan[1][0];
				pan1 = Math.Max(Math.Min(pan1, 6), 1);
				pan2 = Math.Max(Math.Min(pan2, clk), 1);
				pan2 = (pan1 > 3 && pan1 < 7) ? pan2 : DEF.EMPTY;
				gWork.cpyt[i].putpan(0, pan1);
				gWork.cpyt[i].putpan(1, pan2);
				gWork.rp.s_rep();
				return;
			}

			pan1 = rndfil(6) + 1;// 1 ～ 6
			pan2 = (pan1 > 3 && pan1 < 7) ? (rndfil(clk - 1) + 1) : DEF.EMPTY;//1～255
			gWork.cpyt[i].putpan(0, pan1);
			gWork.cpyt[i].putpan(1, pan2);
			gWork.rp.s_rep();

		}

		//------------------------------------------------------------------
		//新しい乱数clkを設定する
		private void newrclk(int i)
		{
			if (i != 0 && gWork.rp.c_clk != 0)
			{
				gWork.cpyt[i].putclk(gWork.cpyt[i - 1].getclk());
				gWork.rp.c_clk--;
				return;
			}

			gWork.cpyt[i].putclk(rndfil(256));// 0 ～ 255
			gWork.rp.s_rep();

		}

		public GD3Tag GetGD3TagInfo(byte[] srcBuf)
		{
			throw new NotImplementedException();
		}
	}
}
