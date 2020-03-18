using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public class strpm
	{
		public string prm = null;
		public strpm next;
		//friend class STRPRM;

		public strpm(int n) //prmに文字列ﾊﾞｯﾌｧのｱﾄﾞﾚｽをｾｯﾄするconstructer
		{
			prm = gWork.com.sptr[n];
			//int a, b;
			//a = b = 0; //gWork.com.sptr;
			//int i;
			////for (i = 0; a<gWork.com.sptr.Length && gWork.com.sptr[a] != ',' && gWork.com.sptr[a] != '>'; i++, a++) ;   //文字数を数える
			//i = gWork.com.sptr.Length;                                                                          //prm=new char[i + 1];            //領域確保
			//a = 0;// prm;
			//prm = "";
			//for (int y = 0; y < i; y++)
			//{
			//	prm += gWork.com.sptr[b++];         //パラメータセット
			//}
			////* a = '\0';
		}

		~strpm()
		{
			prm = null;
		}

	}
}
