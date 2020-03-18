using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public class STRPRM
	{
		private strpm head;

		public string ope_kakko(int x)
		{
			if (x == 0) return head.prm;
			if (x >= gWork.com.spnum) return "";
			strpm p = head.next;
			for (int i = 1; i < x; i++)
			{
				p = p.next;
			}
			return p.prm;
		}

		public STRPRM()
		{
			head = new strpm(0);
			head.next = new strpm(1);
			strpm p = head.next;

			for (int i = 2; i <= gWork.com.spnum; i++)
			{
				p.next = new strpm(i);
				p = p.next;
			}
		}

		~STRPRM()
		{
			strpm p;
			int i;
			for (i = 0, p = head; i < gWork.com.spnum; i++)
			{
				strpm tmp;
				tmp = p.next;
				//delete p;
				p = tmp;
			}
		}

	}
}
