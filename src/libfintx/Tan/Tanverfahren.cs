/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 	
 */

using System;
using System.Collections.Generic;

namespace libfintx
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
                    }
                }
            }

            return list;
        }
    }
}
