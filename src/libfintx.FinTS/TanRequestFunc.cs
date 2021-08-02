/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 - X Marco Silipo
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

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading.Tasks;

namespace libfintx.FinTS
{
    /// <summary>
    /// Helper object needed for entering a TAN.
    /// </summary>
    public  class TanRequestFunc : TanRequest
    {

        public Func<MatrixCode, Task<string>> WithMatrixFunc { get; set; } = (para) => throw new NotImplementedException();
        public Func<FlickerCodeRenderer, Task<string>> WithFlickerFunc { get; set; } = (para) => throw new NotImplementedException();
        public Func<Task<string>> WithUnknownFunc { get; set; } = () => throw new NotImplementedException();


        public Func<Task> OnTransactionEndedFunc { get; set; } = () => Task.CompletedTask;



        public override async Task OnTransactionEndedAsync(bool success) => await OnTransactionEndedFunc();
        public override async Task<string> WithMatrixAsync(MatrixCode matrixCode) => await WithMatrixFunc(matrixCode);
        public override async Task<string> WithFlickerAsync(FlickerCodeRenderer flickerCodeRenderer) => await WithFlickerFunc(flickerCodeRenderer);
        public override async Task<string> WithUnknownAsync() => await WithUnknownFunc();
    }
}
