using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
	public class flgdat
	{
		public flag fnte;
		public flag fvol;
		public flag fqtz;
		public flag flen;
		public flag fvoi;
		public flag fmac;
		public flag fptm;
		public flag fpan;
		public flag fkst;
		public flag fclk;
		public flag fext;

		public flgdat(flag a)
		{
			fnte = fvol = fqtz = flen = fvoi = fmac = fptm = fpan = fkst = fclk = fext = a;
		}

		public int allflgck()
		{
			if (fnte == flag.OFF
				&& fvol == flag.OFF
				&& flen == flag.OFF
				&& fqtz == flag.OFF
				&& fmac == flag.OFF
				&& fvoi == flag.OFF
				&& fptm == flag.OFF
				&& fpan == flag.OFF
				&& fkst == flag.OFF
				&& fclk == flag.OFF
				&& fext == flag.OFF)
			{
				return 0;
			}
			else
			{
				return DEF.ERROR;
			}
		}

		public void allflgof()    //すべてのフラグをoffにする	
		{
			fnte = fvol = fqtz = flen = fvoi = fmac = fptm = fpan =
			fkst = fclk = fext = flag.OFF;
		}

		public void allflgon()    //すべてのフラグをonにする	
		{
			fnte = fvol = fqtz = flen = fvoi = fmac = fptm = fpan =
			fkst = fclk = fext = flag.ON;
		}

		public flgdat Copy()
		{
			flgdat ret = new flgdat(flag.OFF);
			ret.fnte = this.fnte;
			ret.fvol = this.fvol;
			ret.fqtz = this.fqtz;
			ret.flen = this.flen;
			ret.fvoi = this.fvoi;
			ret.fmac = this.fmac;
			ret.fptm = this.fptm;
			ret.fpan = this.fpan;
			ret.fkst = this.fkst;
			ret.fclk = this.fclk;
			ret.fext = this.fext;

			return ret;
		}
	}
}
