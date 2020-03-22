using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public class grp_prm
	{
		private int[] nte = new int[DEF.MAXNTEGR];
		public int[] voi = new int[DEF.MAXVOIGR];
		public int[] len = new int[DEF.MAXLENGR];
		public int[] qtz = new int[DEF.MAXQTZGR];
		public int[] mac = new int[DEF.MAXMACGR];
		public int[] kst = new int[DEF.MAXKSTGR];
		public int[] ext = new int[DEF.MAXEXTGR];
		public int[][] pan = new int[2][] { new int[DEF.MAXPANGR], new int[DEF.MAXPANGR] };

		public void setnte(int a, int n) { if (a <= DEF.MAXNTEGR) nte[a] = n; }
		public int getnte(int n) { return (nte[n]); }
		public void clsnte()
		{
			for (int i = 0; i < DEF.MAXNTEGR; i++) nte[i] = 0;
		}
	}
}
