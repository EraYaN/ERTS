/*******************************************************************\
 *                                                                   *
 *   Library         : lib_crc                                       *
 *   File            : lib_crc.cs                                     *
 *   Author          : Lammert Bies  1999-2008                       *
 *   E-mail          : info@lammertbies.nl                           *
 *   Language        : Visual C#                                     *
 *                                                                   *
 *                                                                   *
 *   Description                                                     *
 *   ===========                                                     *
 *                                                                   *
 *   The file lib_crc.c contains the private  and  public  func-     *
 *   tions  used  for  the  calculation of CRC-16, CRC-CCITT and     *
 *   CRC-32 cyclic redundancy values.                                *
 *                                                                   *
 *                                                                   *
 *   Modification history                                            *
 *   ====================                                            *
 *                                                                   *
 *   Date        Version Comment                                     *
 *                                                                   *
 *   2012                Translated to C# (JP)                       *
 *   2010-10-24  2.0     Rewritten to add more algos (JP)            *
 *   2010-10-20  1.17    Added several routines (JP)                 *
 *   2008-04-20  1.16    Added CRC-CCITT calculation for Kermit      *
 *                                                                   *
 *   2007-04-01  1.15    Added CRC16 calculation for Modbus          *
 *                                                                   *
 *   2007-03-28  1.14    Added CRC16 routine for Sick devices        *
 *                                                                   *
 *   2005-12-17  1.13    Added CRC-CCITT with initial 0x1D0F         *
 *                                                                   *
 *   2005-05-14  1.12    Added CRC-CCITT with start value 0          *
 *                                                                   *
 *   2005-02-05  1.11    Fixed bug in CRC-DNP routine                *
 *                                                                   *
 *   2005-02-04  1.10    Added CRC-DNP routines                      *
 *                                                                   *
 *   1999-02-21  1.01    Added FALSE and TRUE mnemonics              *
 *                                                                   *
 *   1999-01-22  1.00    Initial source                              *
 *                                                                   *
\*******************************************************************/

/*******************************************************************\
 *                                                                   *
 *   #define P_xxxx                                                  *
 *                                                                   *
 *   The CRC's are computed using polynomials. The  coefficients     *
 *   for the algorithms are defined by the following constants.      *
 *                                                                   *
\*******************************************************************/

/*******************************************************************\
 *                                                                   *
 *   #define P_xxxx                                                  *
 *                                                                   *
 *   The CRC's are computed using polynomials. The  coefficients     *
 *   for the algorithms are defined by the following constants.      *
 *                                                                   *
\*******************************************************************/
/*
  From this site:
  http://www.nationmaster.com/encyclopedia/Cyclic-redundancy-check#Computation_of_CRC

  CRC-8-ATM 	        x8 + x2 + x + 1             (ATM HEC) 	    0x07 or 0xE0 (0xC1)
  CRC-8-CCITT 	        x8 + x7 + x3 + x2 + 1       (1-Wire bus) 	0x8D or 0xB1 (0x63)
  CRC-8-Dallas/Maxim 	x8 + x5 + x4 + 1            (1-Wire bus) 	0x31 or 0x8C (0x19)
  CRC-8 	            x8 + x7 + x6 + x4 + x2 + 1 	                0xD5 or 0xAB (0x57)
  CRC-8-SAE J1850 	    x8 + x4 + x3 + x2 + 1 	                    0x1D or 0xB8 (0x71)

  From
  http://en.wikipedia.org/wiki/Cyclic_redundancy_check
  CRC-8-WCDMA         x8 + x7 + x4 + x3 + x + 1                   0x9B

  Results verified with this CRC calculator:
  http://www.zorc.breitbandkatze.de/crc.html
*/

namespace CRCLib
{

    public class crclib
    {
        const byte P_8_ATM = 0x07;
        const byte P_8_CCITT = 0x8D;
        const byte P_8_MAXIM = 0x8C;    ///* Maxim: reflected 31 */
        const byte P_8 = 0xD5;
        const byte P_8_J1850 = 0x1D;
        const byte P_8_WCDMA = 0xD9;    ///* WCDMA: reflected 9B */
        const byte P_8_ROHC = 0xE0;     ///* ROHC: reflected 07 */
        const byte P_8_DARC = 0x9C;     ///* DARC: reflected 39 */

        const ushort P_16_NORMAL = 0x8005;
        const ushort P_16_REFLECTED = 0xA001; /* 16: 8005 reflected */
        const ushort P_CCITT = 0x1021;

        /* DNP: 3D65 reflected */
        const ushort P_DNP = 0xA6BC;
        const ushort P_EN_13757 = 0x3D65;

        /* Kermit: 1021 reflected */
        const ushort P_KERMIT = 0x8408;
        const ushort P_SICK = 0x8005;

        const ushort P_T10_DIF = 0x8BB7;
        const ushort P_DECT = 0x0589;
        const ushort P_TELEDISK = 0xA097;

        const uint P_24 = 0x5D6DCB;
        const uint P_24_R64 = 0x864CFB;

        const uint P_32_NORMAL = 0x04C11DB7;
        /* 32: 04C11DB7 reflected */
        const uint P_32_REFLECTED = 0xEDB88320;
        /* C: 1EDC6F41 reflected */
        const uint P_32C = 0x82F63B78;

        /* D: A833982B reflected */
        const uint P_32D = 0xD419CC15;
        const uint P_32K = 0x741B8CD7;
        const uint P_32Q = 0x814141AB;

        const uint P_32_XFER = 0x000000AF;

        const ulong P_40_GSM = 0x0004820009L;
        const ulong P_64_NORMAL = 0x42F0E1EBA9EA3693L;

        /* 1B:    000000000000001B reflected */
        const ulong P_64_1B_REFL = 0xD800000000000000L;
        /* Jones: AD93D23594C935A9 reflected */
        const ulong P_64_JONES_REFL = 0x95AC9329AC4BC9B5L;

        private byte[] TestString = { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39 };

        private byte[] crc8_table_atm = new byte[256];
        private byte[] crc8_table_ccitt = new byte[256];
        private byte[] crc8_table_maxim = new byte[256];
        private byte[] crc8_table = new byte[256];
        private byte[] crc8_table_j1850 = new byte[256];
        private byte[] crc8_table_wcdma = new byte[256];
        private byte[] crc8_table_rohc = new byte[256];
        private byte[] crc8_table_darc = new byte[256];

        private ulong[] crc_tab641b_reflected = new ulong[256];
        private ulong[] crc_tab64jones_reflected = new ulong[256];
        private ulong[] crc_tab64_normal = new ulong[256];
        private ulong[] crc_tab40gsm_normal = new ulong[256];
        private uint[] crc_tab32_normal = new uint[256];
        private uint[] crc_tab32_reflected = new uint[256];
        private uint[] crc_tabxfer_normal = new uint[256];
        private uint[] crc_tab32C = new uint[256];
        private uint[] crc_tab32D = new uint[256];
        private uint[] crc_tab32K = new uint[256];
        private uint[] crc_tab32Q = new uint[256];
        private uint[] crc_tab24 = new uint[256];
        private uint[] crc_tab24r64 = new uint[256];
        private ushort[] crc_tab_8005_normal = new ushort[256];
        private ushort[] crc_tab_8005_reflected = new ushort[256];
        private ushort[] crc_tab_1021_normal = new ushort[256];
        private ushort[] crc_tab_1021_reflected = new ushort[256];
        private ushort[] crc_tab_3d65_normal = new ushort[256];
        private ushort[] crc_tab_3d65_reflected = new ushort[256];
        private ushort[] crc_tabt10dif = new ushort[256];
        private ushort[] crc_tabdect = new ushort[256];
        private ushort[] crc_tabteledisk = new ushort[256];

        public static ushort reverse_endian(ushort i)
        {
            return (ushort)((i >> 8) | ((i & 0xFF) << 8));
        }

        public void init_crc8_normal_tab(byte[] table, byte polynom)
        {
            uint i, j;
            byte crc8;

            for (i = 0; i < 256; i++)
            {
                crc8 = (byte)i;

                for (j = 0; j < 8; j++)
                {
                    if ((crc8 & 0x80) != 0)
                        crc8 = (byte)((crc8 << 1) ^ polynom);
                    else
                        crc8 <<= 1;
                }
                table[i] = crc8;
            }
        }

        public void init_crc8_reflected_tab(byte[] table, byte polynom)
        {
            uint i, j;
            byte crc8;

            for (i = 0; i < 256; i++)
            {
                crc8 = (byte)i;

                for (j = 0; j < 8; j++)
                {
                    if ((crc8 & 0x01) != 0)
                        crc8 = (byte)((crc8 >> 1) ^ polynom);
                    else
                        crc8 >>= 1;
                }
                table[i] = crc8;
            }
        }

        public void init_crc16_normal_tab(ushort[] table, ushort polynom)
        {
            uint i, j;
            ushort crc16;

            for (i = 0; i < 256; i++)
            {
                crc16 = (ushort)(i << 8);

                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x8000) != 0)
                        crc16 = (ushort)((crc16 << 1) ^ polynom);
                    else
                        crc16 <<= 1;
                }
                table[i] = crc16;
            }
        }

        public void init_crc16_reflected_tab(ushort[] table, ushort polynom)
        {
            uint i, j;
            ushort crc16;

            for (i = 0; i < 256; i++)
            {
                crc16 = (ushort)i;

                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x0001) != 0)
                        crc16 = (ushort)((crc16 >> 1) ^ polynom);
                    else
                        crc16 >>= 1;
                }
                table[i] = crc16;
            }
        }

        public void init_crc24_normal_tab(uint[] table, uint polynom)
        {
            uint i, j;
            uint crc24;

            for (i = 0; i < 256; i++)
            {
                crc24 = i << 16;

                for (j = 0; j < 8; j++)
                {
                    if ((crc24 & 0x00800000) != 0)
                        crc24 = (crc24 << 1) ^ polynom;
                    else
                        crc24 <<= 1;
                }
                table[i] = crc24;
            }
        }

        public void init_crc32_normal_tab(uint[] table, uint polynom)
        {
            uint i, j;
            uint crc32;

            for (i = 0; i < 256; i++)
            {
                crc32 = i << 24;

                for (j = 0; j < 8; j++)
                {
                    if ((crc32 & 0x80000000) != 0)
                        crc32 = (crc32 << 1) ^ polynom;
                    else
                        crc32 <<= 1;
                }
                table[i] = crc32;
            }
        }

        public void init_crc32_reflected_tab(uint[] table, uint polynom)
        {
            uint i, j;
            uint crc32;

            for (i = 0; i < 256; i++)
            {
                crc32 = i;

                for (j = 0; j < 8; j++)
                {
                    if ((crc32 & 0x00000001) != 0)
                        crc32 = (crc32 >> 1) ^ polynom;
                    else
                        crc32 >>= 1;
                }
                table[i] = crc32;
            }
        }

        public void init_crc40_normal_tab(ulong[] table, ulong polynom)
        {
            ulong i;
            uint j;
            ulong crc40;

            for (i = 0; i < 256; i++)
            {
                crc40 = i << 32;

                for (j = 0; j < 8; j++)
                {
                    if ((crc40 & 0x0000008000000000L) != 0)
                        crc40 = (crc40 << 1) ^ polynom;
                    else
                        crc40 <<= 1;
                }
                table[i] = crc40;
            }
        }

        public void init_crc64_normal_tab(ulong[] table, ulong polynom)
        {
            ulong i;
            uint j;
            ulong crc64;

            for (i = 0; i < 256; i++)
            {
                crc64 = i << 56;

                for (j = 0; j < 8; j++)
                {
                    if ((crc64 & 0x8000000000000000L) != 0)
                        crc64 = (crc64 << 1) ^ polynom;
                    else
                        crc64 <<= 1;
                }
                table[i] = crc64;
            }
        }

        public void init_crc64_reflected_tab(ulong[] table, ulong polynom)
        {
            ulong i;
            uint j;
            ulong crc64;

            for (i = 0; i < 256; i++)
            {
                crc64 = i;

                for (j = 0; j < 8; j++)
                {
                    if ((crc64 & 0x0000000000000001L) != 0)
                        crc64 = (crc64 >> 1) ^ polynom;
                    else
                        crc64 >>= 1;
                }
                table[i] = crc64;
            }
        }

        public crclib()
        {
            init_crc8_normal_tab(crc8_table_atm, P_8_ATM);
            init_crc8_normal_tab(crc8_table_ccitt, P_8_CCITT);
            init_crc8_reflected_tab(crc8_table_maxim, P_8_MAXIM);
            init_crc8_normal_tab(crc8_table, P_8);
            init_crc8_normal_tab(crc8_table_j1850, P_8_J1850);
            init_crc8_reflected_tab(crc8_table_wcdma, P_8_WCDMA);
            init_crc8_reflected_tab(crc8_table_rohc, P_8_ROHC);
            init_crc8_reflected_tab(crc8_table_darc, P_8_DARC);
            init_crc16_normal_tab(crc_tab_1021_normal, P_CCITT);
            init_crc16_reflected_tab(crc_tab_1021_reflected, P_KERMIT);
            init_crc16_normal_tab(crc_tab_8005_normal, P_16_NORMAL);
            init_crc16_reflected_tab(crc_tab_8005_reflected, P_16_REFLECTED);
            init_crc16_normal_tab(crc_tab_3d65_normal, 0x3D65);
            init_crc16_reflected_tab(crc_tab_3d65_reflected, P_DNP);
            init_crc16_normal_tab(crc_tabt10dif, P_T10_DIF);
            init_crc16_normal_tab(crc_tabdect, P_DECT);
            init_crc16_normal_tab(crc_tabteledisk, P_TELEDISK);

            init_crc24_normal_tab(crc_tab24, P_24);
            init_crc24_normal_tab(crc_tab24r64, P_24_R64);

            init_crc32_reflected_tab(crc_tab32_reflected, P_32_REFLECTED);
            init_crc32_normal_tab(crc_tab32_normal, P_32_NORMAL);
            init_crc32_normal_tab(crc_tabxfer_normal, P_32_XFER);
            init_crc32_reflected_tab(crc_tab32C, P_32C);
            init_crc32_reflected_tab(crc_tab32D, P_32D);
            init_crc32_normal_tab(crc_tab32K, P_32K); /* Not sure */
            init_crc32_normal_tab(crc_tab32Q, P_32Q);

            init_crc40_normal_tab(crc_tab40gsm_normal, P_40_GSM);
            init_crc64_normal_tab(crc_tab64_normal, P_64_NORMAL);
            init_crc64_reflected_tab(crc_tab641b_reflected, P_64_1B_REFL);
            init_crc64_reflected_tab(crc_tab64jones_reflected, P_64_JONES_REFL);
        }

        void init_crc8_atm_tab()
        {
            init_crc8_normal_tab(crc8_table_atm, P_8_ATM);
        }

        void init_crc8_ccitt_tab()
        {
            init_crc8_normal_tab(crc8_table_ccitt, P_8_CCITT);
        }

        void init_crc8_maxim_tab()
        {
            init_crc8_reflected_tab(crc8_table_maxim, P_8_MAXIM);
        }

        void init_crc8_tab()
        {
            init_crc8_normal_tab(crc8_table, P_8);
        }

        void init_crc8_j1850_tab()
        {
            init_crc8_normal_tab(crc8_table_j1850, P_8_J1850);
        }

        void init_crc8_wcdma_tab()
        {
            init_crc8_reflected_tab(crc8_table_wcdma, P_8_WCDMA);
        }

        void init_crc8_rohc_tab()
        {
            init_crc8_reflected_tab(crc8_table_rohc, P_8_ROHC);
        }

        void init_crc8_darc_tab()
        {
            init_crc8_reflected_tab(crc8_table_darc, P_8_DARC);
        }


        public byte update_crc8_atm(byte crc, byte c)
        {
            return (crc8_table_atm[crc ^ c]);
        }

        public byte update_crc8_ccitt(byte crc, byte c)
        {
            return (crc8_table_ccitt[crc ^ c]);
        }

        public byte update_crc8_maxim(byte crc, byte c)
        {
            return (crc8_table_maxim[crc ^ c]);
        }

        public byte update_crc8(byte crc, byte c)
        {
            return (crc8_table[crc ^ c]);
        }

        public byte update_crc8_j1850(byte crc, byte c)
        {
            return (crc8_table_j1850[crc ^ c]);
        }

        public byte update_crc8_wcdma(byte crc, byte c)
        {
            return (crc8_table_wcdma[crc ^ c]);
        }

        public byte update_crc8_rohc(byte crc, byte c)
        {
            return (crc8_table_rohc[crc ^ c]);
        }

        public byte update_crc8_darc(byte crc, byte c)
        {
            return (crc8_table_darc[crc ^ c]);
        }

        public byte calculate_crc8_itu(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_atm(crc8, p[i]);
            }
            return (byte)(crc8 ^ 0x55);
        }

        public byte calculate_crc8_atm(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_atm(crc8, p[i]);
            }
            return crc8;
        }

        public byte calculate_crc8_ccitt(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_ccitt(crc8, p[i]);
            }
            return crc8;
        }

        public byte calculate_crc8_maxim(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_maxim(crc8, p[i]);
            }
            return crc8;
        }

        public byte calculate_crc8(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8(crc8, p[i]);
            }
            return crc8;
        }

        public byte calculate_crc8_icode(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0xFD;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_j1850(crc8, p[i]);
            }
            return crc8;
        }

        public byte calculate_crc8_j1850(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0xFF;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_j1850(crc8, p[i]);
            }
            return (byte)(~crc8);
        }

        public byte calculate_crc8_wcdma(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_wcdma(crc8, p[i]);
            }
            return crc8;
        }

        public byte calculate_crc8_rohc(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0xFF;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_rohc(crc8, p[i]);
            }
            return crc8;
        }

        public byte calculate_crc8_darc(byte[] p, uint length)
        {
            byte crc8;
            uint i;

            crc8 = 0;

            for (i = 0; i < length; i++)
            {
                crc8 = update_crc8_darc(crc8, p[i]);
            }
            return crc8;
        }


        /* Common routines for calculations */
        public ushort update_crc16_normal(ushort[] table, ushort crc, byte c)
        {
            ushort short_c;

            short_c = (ushort)(0x00ff & c);

            /* Normal form */
            return (ushort)((crc << 8) ^ table[(crc >> 8) ^ short_c]);
        }

        public ushort update_crc16_reflected(ushort[] table, ushort crc, byte c)
        {
            ushort short_c;

            short_c = (ushort)(0x00ff & c);

            /* Reflected form */
            return (ushort)((crc >> 8) ^ table[(crc ^ short_c) & 0xff]);
        }

        public uint update_crc24_normal(uint[] table, uint crc, byte c)
        {
            uint long_c;

            long_c = 0x000000ff & (uint)c;

            return (crc << 8) ^ table[((crc >> 16) ^ long_c) & 0xff];
        }

        public uint update_crc32_normal(uint[] table, uint crc, byte c)
        {
            uint long_c;

            long_c = 0x000000ff & (uint)c;

            return (crc << 8) ^ table[((crc >> 24) ^ long_c) & 0xff];
        }

        public uint update_crc32_reflected(uint[] table, uint crc, byte c)
        {
            uint long_c;

            long_c = 0x000000ff & (uint)c;

            return (crc >> 8) ^ table[(crc ^ long_c) & 0xff];
        }

        public ulong update_crc40_normal(ulong[] table, ulong crc, byte c)
        {
            ulong long64_c;

            long64_c = 0x00000000000000ffL & (ulong)c;

            return (crc << 8) ^ table[((crc >> 32) ^ long64_c) & 0xff];
        }

        public ulong update_crc64_normal(ulong[] table, ulong crc, byte c)
        {
            ulong long64_c;

            long64_c = 0x00000000000000ffL & (ulong)c;

            return (crc << 8) ^ table[((crc >> 56) ^ long64_c) & 0xff];
        }

        public ulong update_crc64_reflected(ulong[] table, ulong crc, byte c)
        {
            ulong long64_c;

            long64_c = 0x00000000000000ffL & (ulong)c;

            return (crc >> 8) ^ table[(crc ^ long64_c) & 0xff];
        }

        public ushort update_crc_sick(ushort crc, byte c, byte prev_byte)
        {
            ushort short_c, short_p;

            short_c = c;
            short_p = (ushort)(prev_byte << 8);

            if ((crc & 0x8000) != 0)
                crc = (ushort)((crc << 1) ^ P_SICK);
            else
                crc = (ushort)(crc << 1);

            crc &= 0xffff;
            crc ^= (ushort)(short_c | short_p);

            return crc;
        }

        public ushort update_crc16_8005(ushort crc, byte c)
        {
            return update_crc16_normal(crc_tab_8005_normal, crc, c);
        }
        public ushort update_crc16_A001(ushort crc, byte c)
        {
            return update_crc16_reflected(crc_tab_8005_reflected, crc, c);
        }
        public ushort update_crc16_1021(ushort crc, byte c)
        {
            return update_crc16_normal(crc_tab_1021_normal, crc, c);
        }
        public ushort update_crc16_8408(ushort crc, byte c)
        {
            return update_crc16_reflected(crc_tab_1021_reflected, crc, c);
        }
        public ushort update_crc16_3D65(ushort crc, byte c)
        {
            return update_crc16_normal(crc_tab_3d65_normal, crc, c);
        }
        public ushort update_crc16_dnp(ushort crc, byte c)
        {
            return update_crc16_reflected(crc_tab_3d65_reflected, crc, c);
        }
        public ushort update_crc16_t10_dif(ushort crc, byte c)
        {
            return update_crc16_normal(crc_tabt10dif, crc, c);
        }
        public ushort update_crc16_0589(ushort crc, byte c)
        {
            return update_crc16_normal(crc_tabdect, crc, c);
        }
        public ushort update_crc16_teledisk(ushort crc, byte c)
        {
            return update_crc16_normal(crc_tabteledisk, crc, c);
        }
        public uint update_crc24(uint crc, byte c)
        {
            return update_crc24_normal(crc_tab24, crc, c);
        }
        public uint update_crc24_r64(uint crc, byte c)
        {
            return update_crc24_normal(crc_tab24r64, crc, c);
        }
        public uint update_crc32_refl(uint crc, byte c)
        {
            return update_crc32_reflected(crc_tab32_reflected, crc, c);
        }
        public uint update_crc32_norm(uint crc, byte c)
        {
            return update_crc32_normal(crc_tab32_normal, crc, c);
        }
        public uint update_crc32_xfer(uint crc, byte c)
        {
            return update_crc32_normal(crc_tabxfer_normal, crc, c);
        }
        public uint update_crc32_c(uint crc, byte c)
        {
            return update_crc32_reflected(crc_tab32C, crc, c);
        }
        public uint update_crc32_d(uint crc, byte c)
        {
            return update_crc32_reflected(crc_tab32D, crc, c);
        }
        public uint update_crc32_k(uint crc, byte c)
        {
            return update_crc32_normal(crc_tab32K, crc, c);
        }
        public uint update_crc32_q(uint crc, byte c)
        {
            return update_crc32_normal(crc_tab32Q, crc, c);
        }

        public ulong update_crc40_gsm(ulong crc, byte c)
        {
            return update_crc40_normal(crc_tab40gsm_normal, crc, c);
        }

        public ulong update_crc64(ulong crc, byte c)
        {
            return update_crc64_normal(crc_tab64_normal, crc, c);
        }

        public ulong update_crc64_1B(ulong crc, byte c)
        {
            return update_crc64_reflected(crc_tab641b_reflected, crc, c);
        }

        public ulong update_crc64_jones(ulong crc, byte c)
        {
            return update_crc64_reflected(crc_tab64jones_reflected, crc, c);
        }

        public ushort calculate_crc16_Buypass(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_8005(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_DDS_110(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0x800D;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_8005(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_EN_13757(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_3D65(crc, p[i]);
            }
            return (ushort)(~crc);
        }


        public ushort calculate_crc16_Teledisk(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_teledisk(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_A001(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Modbus(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0xFFFF;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_A001(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Maxim(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_A001(crc, p[i]);
            }
            return (ushort)(~crc);
        }

        public ushort calculate_crc16_USB(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0xFFFF;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_A001(crc, p[i]);
            }
            return (ushort)(~crc);
        }

        public ushort calculate_crc16_T10_DIF(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_t10_dif(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Dect_X(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_0589(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Dect_R(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_0589(crc, p[i]);
            }
            return (ushort)(crc ^ 0x0001);
        }

        public ushort calculate_crc16_sick(byte[] p, uint length)
        {
            ushort crc;
            uint i;
            byte Prev_Byte = 0;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc_sick(crc, p[i], Prev_Byte);
                Prev_Byte = p[i];
            }
            return crclib.reverse_endian(crc);
        }

        public ushort calculate_crc16_DNP(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_dnp(crc, p[i]);
            }
            crc = (ushort)(~crc);

            return crclib.reverse_endian(crc);
        }

        public ushort calculate_crc16_Ccitt_Xmodem(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_1021(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Ccitt_FFFF(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0xFFFF;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_1021(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Ccitt_1D0F(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0x1D0F;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_1021(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Genibus(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0xFFFF;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_1021(crc, p[i]);
            }
            return (ushort)(~crc);
        }

        public ushort calculate_crc16_Kermit(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_8408(crc, p[i]);
            }

            return crclib.reverse_endian(crc);
        }

        public ushort calculate_crc16_X25(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0xFFFF;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_8408(crc, p[i]);
            }
            return (ushort)(~crc);
        }

        public ushort calculate_crc16_MCRF4XX(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0xFFFF;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_8408(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_crc16_Riello(byte[] p, uint length)
        {
            ushort crc;
            uint i;

            crc = 0x554D;

            for (i = 0; i < length; i++)
            {
                crc = update_crc16_8408(crc, p[i]);
            }
            return crc;
        }

        public ushort calculate_chk16_Fletcher(byte[] p, uint length)
        {
            ushort check, check_fletcher;
            uint i;

            check = 0;
            check_fletcher = 0;

            for (i = 0; i < length; i++)
            {
                check += (p[i]);
                check_fletcher += check;
            }
            return (ushort)(((check_fletcher & 0xFF) << 8) | (check & 0xFF));
        }

        public uint calculate_crc24_flexray_a(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0x00FEDCBA;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc24(crc32, p[i]);
            }

            return crc32;
        }

        public uint calculate_crc24_flexray_b(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0x00ABCDEF;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc24(crc32, p[i]);
            }

            return crc32;
        }

        public uint calculate_crc24_r64(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0x00B704CE;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc24_r64(crc32, p[i]);
            }

            return crc32;
        }

        public uint calculate_crc32(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0xFFFFFFFF;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_refl(crc32, p[i]);
            }

            /* One's complement = Xor with FFFFFFFF */
            return ~crc32;
        }

        public uint calculate_crc32_jamcrc(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0xFFFFFFFF;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_refl(crc32, p[i]);
            }

            return crc32;
        }

        public uint calculate_crc32_c(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0xFFFFFFFF;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_c(crc32, p[i]);
            }

            /* One's complement = Xor with FFFFFFFF */
            return ~crc32;
        }

        public uint calculate_crc32_d(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0xFFFFFFFF;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_d(crc32, p[i]);
            }

            /* One's complement = Xor with FFFFFFFF */
            return ~crc32;
        }

        public uint calculate_crc32_bzip2(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0xFFFFFFFF;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_norm(crc32, p[i]);
            }

            /* One's complement = Xor with FFFFFFFF */
            return ~crc32;
        }

        public uint calculate_crc32_mpeg2(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0xFFFFFFFF;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_norm(crc32, p[i]);
            }

            return crc32;
        }

        public uint calculate_crc32_posix(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_norm(crc32, p[i]);
            }

            /* One's complement = Xor with FFFFFFFF */
            return ~crc32;
        }

        public uint calculate_crc32_k(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_k(crc32, p[i]);
            }

            return crc32;
        }

        public uint calculate_crc32_q(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_q(crc32, p[i]);
            }

            return crc32;
        }

        public uint calculate_crc32_xfer(byte[] p, uint length)
        {
            uint crc32;
            uint i;

            crc32 = 0;

            for (i = 0; i < length; i++)
            {
                crc32 = update_crc32_xfer(crc32, p[i]);
            }

            return crc32;
        }

        /* from: http://en.wikipedia.org/wiki/Fletcher%27s_checksum#Fletcher-32 */
        public uint calculate_fletcher32(ushort[] data, uint words)
        {
            uint tlen, i = 0;
            uint sum1 = 0xffff, sum2 = 0xffff;

            while (words != 0)
            {
                tlen = words > 359 ? 359 : words;
                words -= tlen;
                do
                {
                    sum2 += sum1 += data[i++];
                } while (--tlen != 0);
                sum1 = (sum1 & 0xffff) + (sum1 >> 16);
                sum2 = (sum2 & 0xffff) + (sum2 >> 16);
            }
            /* Second reduction step to reduce sums to 16 bits */
            sum1 = (sum1 & 0xffff) + (sum1 >> 16);
            sum2 = (sum2 & 0xffff) + (sum2 >> 16);
            return sum2 << 16 | sum1;
        }

        public ulong calculate_crc40_gsm(byte[] p, uint length)
        {
            ulong crc64;
            uint i;

            crc64 = 0L;

            for (i = 0; i < length; i++)
            {
                crc64 = update_crc40_gsm(crc64, p[i]);
            }

            return crc64;
        }

        public ulong calculate_crc64(byte[] p, uint length)
        {
            ulong crc64;
            uint i;

            crc64 = 0L;

            for (i = 0; i < length; i++)
            {
                crc64 = update_crc64(crc64, p[i]);
            }

            return crc64;
        }

        public ulong calculate_crc64_1b(byte[] p, uint length)
        {
            ulong crc64;
            uint i;

            crc64 = 0L;

            for (i = 0; i < length; i++)
            {
                crc64 = update_crc64_1B(crc64, p[i]);
            }

            return crc64;
        }

        public ulong calculate_crc64_we(byte[] p, uint length)
        {
            ulong crc64;
            uint i;

            crc64 = 0xFFFFFFFFFFFFFFFFL;

            for (i = 0; i < length; i++)
            {
                crc64 = update_crc64(crc64, p[i]);
            }

            /* One's complement = Xor with FFFFFFFFFFFFFFFF */
            return ~crc64;
        }

        public ulong calculate_crc64_jones(byte[] p, uint length)
        {
            ulong crc64;
            uint i;

            crc64 = 0xFFFFFFFFFFFFFFFFL;

            for (i = 0; i < length; i++)
            {
                crc64 = update_crc64_jones(crc64, p[i]);
            }

            return crc64;
        }
    }
}
