using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public class MMLDAT
	{
		public linedat[] head;

		public MMLDAT(string a, ref linedat[] ld)
		{
			int line;       //行数カウンター
			string[] ary = a.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			if (a.Length>1 && a.Substring(a.Length - 2, 2) == "\r\n")
			{
				List<string> aa = ary.ToList();
				aa.RemoveAt(aa.Count - 1);
				ary = aa.ToArray();
			}

			line = ary.Length;
			head = new linedat[line];
			for (int i = 0; i < ary.Length; i++)
			{
				sep(ary[i], ref head[i]);
			}
			ld = head;
		}

		~MMLDAT()
		{
			head = null;
		}

		private int sep(string a, ref linedat p)
		{
			p = new linedat();
			p.chheader = null;
			p.mmldat = a;
			if (string.IsNullOrEmpty(a)) return -1;
			if (a.Length < 1) return -1;

			int index = 0;
			//while(index<a.Length && (a[index] == ' '|| a[index] == 0x9)) { index++; }

			if (a[index] == '#')
			{
				if (a.Length < 2 + index) return -1;
				int ptr = 1 + index;
				while (ptr < a.Length && (a[ptr] == ' ' || a[ptr] == 0x9)) { ptr++; }
				if (ptr == a.Length) return -1;
				if (a[ptr] != '*') return -1;
			}
			else if (
				(a[index] < 'A' || a[index] > 'Z')
				&& a[index] != '{'
				&& a[index] != '}')
			{
				return -1;
			}

			p.mmldat = "";
			p.chheader = a[index].ToString();
			if (a.Length < 2 + index) return -1;
			p.mmldat = a.Substring(1 + index);
			return 0;
		}

	}
}
