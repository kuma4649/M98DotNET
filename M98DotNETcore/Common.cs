using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public static class Common
	{
		//------------------------------------------------------------------
		//マクロ行サーチ
		public static int searchm(int n)
		{
			for (int i = 0; i < gWork.mml.Length ; i++)
			{
				if (gWork.mml[i].chheader == "#")
				{
					int p = 0;
					while (p < gWork.mml[i].mmldat.Length && gWork.mml[i].mmldat[p] == ' ') { p++; }
					if (gWork.mml[i].mmldat[p] != '*') return DEF.ERROR;
					p++;
					if (atoi(gWork.mml[i].mmldat, p) == n)
					{
						return i;
					}
				}
			}
			return DEF.ERROR;
		}

		public static void initprg()
		{
			for (int i = 0; i <= (DEF.MAXPRG * 512) - 1; i++)
			{
				gWork.progress[i] = i;
				gWork.cpyprg[i] = i;
			}
		}

		public static int atoi(string a, int p)
		{
			string ans = "";
			bool minus = false;
			if (p >= a.Length) return 0;
			if (a[p] == '+') p++;
			else if (a[p] == '-') { p++; minus = true; }

			while (isdigit(a, p))
			{
				ans += a[p++];
			}

			if (ans == "") return 0;
			return int.Parse(ans) * (minus ? -1 : 1);
		}

		public static bool isdigit(string a, int p)
		{
			if (p >= a.Length) return false;
			if (a[p] >= '0' && a[p] <= '9') return true;
			return false;
		}

		//---------------------------------------------------------------------------
		//x行からy行までのMMLを総括してsndbuf1にいれる

		public static void lnkdat(int x, int y)
		{
			int a = 0;
			string b = "";// gWork.sndbuf1;
			int i = 0;
			while (x != y)
			{
				//a = gWork.mml[x].mmldat;
				while (gWork.mml[x].mmldat.Length != a && gWork.mml[x].mmldat[a] != '\n')
				{
					b += gWork.mml[x].mmldat[a++];
					i++;
					if (i > DEF.MAXBUF * 1024)
					{
						DEF.PF("\nError:変換バッファオーバー");
						throw new M98Exception();
					}
				}
				x++;
			}
			//b += '\n';
			gWork.sndbuf1 = b;
		}

	}
}
