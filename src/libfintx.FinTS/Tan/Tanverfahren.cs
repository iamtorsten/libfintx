/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.Collections.Generic;

namespace libfintx.FinTS
{
    class Tanverfahren
    {
        /// <summary>
        /// Verfügbare TAN-Verfahren
        /// </summary>
        /// <param name="HIRMSf"></param>
        /// <returns></returns>
        public List<TanProcess> Parse(String HIRMSf)
        {
            List<TanProcess> list = new List<TanProcess>();

            if (!String.IsNullOrEmpty(HIRMSf))
            {
                var process = HIRMSf.Split(';');

                foreach (var item in process)
                {
                    switch (item)
                    {
                        case "900": // iTAN
                            list.Add(new TanProcess { ProcessNumber = "900", ProcessName = "iTAN" });
                            break;
                        case "910": // chipTAN manuell
                            list.Add(new TanProcess { ProcessNumber = "910", ProcessName = "chipTAN manuell" });
                            break;
                        case "911": // chipTAN optisch
                            list.Add(new TanProcess { ProcessNumber = "911", ProcessName = "chipTAN optisch" });
                            break;
                        case "912": // chipTAN USB
                            list.Add(new TanProcess { ProcessNumber = "912", ProcessName = "chipTAN USB" });
                            break;
                        case "920": // smsTAN
                            list.Add(new TanProcess { ProcessNumber = "920", ProcessName = "smsTAN" });
                            break;
                        case "921": // pushTAN
                            list.Add(new TanProcess { ProcessNumber = "921", ProcessName = "pushTAN" });
                            break;
                        case "922": // pushTAN decoupled
                            list.Add(new TanProcess { ProcessNumber = "922", ProcessName = "pushTAN 2.0" });
                            break;
                        case "942": // mobile-TAN
                            list.Add(new TanProcess { ProcessNumber = "942", ProcessName = "mobile-TAN" });
                            break;
                        case "944": // SecureGo
                            list.Add(new TanProcess { ProcessNumber = "944", ProcessName = "SecureGo" });
                            break;
                        case "962": // Sm@rt-TAN plus manuell
                            list.Add(new TanProcess { ProcessNumber = "962", ProcessName = "Sm@rt-TAN plus manuell" });
                            break;
                        case "972": // Smart-TAN plus optisch
                            list.Add(new TanProcess { ProcessNumber = "972", ProcessName = "Sm@rt-TAN plus optisch" });
                            break;
                        case "982": // photo-TAN
                            list.Add(new TanProcess { ProcessNumber = "982", ProcessName = "photo-TAN" });
                            break;
                        case "994": // chipTAN (Manuell)
                            list.Add(new TanProcess { ProcessNumber = "994", ProcessName = "chipTAN (Manuell)" });
                            break;
                        case "995": // chipTAN (Flicker)
                            list.Add(new TanProcess { ProcessNumber = "995", ProcessName = "chipTAN (Flicker)" });
                            break;
                        case "996": // mobileTAN
                            list.Add(new TanProcess { ProcessNumber = "996", ProcessName = "mobileTAN" });
                            break;
                    }
                }
            }

            return list;
        }
    }
}
