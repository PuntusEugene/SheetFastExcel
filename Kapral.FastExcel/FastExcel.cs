﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace Kapral.FastExcel
{
    public class FastExcel : IFastExcel
    {
        private readonly Application _app;
        private readonly Workbook _workBook;
        private readonly SheetExcelFactory _sheetFactory;

        public FastExcel()
        {
            _app = new Application();
            _workBook = _app.Workbooks.Add(Type.Missing);
            _sheetFactory = new DefaultSheetExcelFactory();
        }

        public FastExcel(SheetExcelFactory sheetFactory) : this()
        {
            _sheetFactory = sheetFactory;
        }

        public FastExcel(string filePath)
        {
            _app = new Application();
            _workBook = _app.Workbooks.Add(filePath);
            _sheetFactory = new DefaultSheetExcelFactory();
        }

        public FastExcel(string filePath, SheetExcelFactory sheetFactory) : this(filePath)
        {
            _sheetFactory = sheetFactory;
        }

        public IEnumerable<ISheetFastExcel> Sheets
        {
            get
            {
                for (int i = 1; i <= _workBook.Worksheets.Count; i++)
                {
                    var sheet = _workBook.Worksheets[i] as Worksheet;
                    yield return _sheetFactory.Get(sheet);
                }
            }
        }

        public void GenerateAndOpen()
        {
            _app.Visible = true;
        }

        public void Save()
        {
            _workBook.Save();
        }

        public void SaveAs(string fileName)
        {
            _app.DisplayAlerts = false;
            _workBook.Saved = true;
            _workBook.SaveAs(fileName);
        }

        /// <summary>
        /// Under certain conditions, "Save" and "SaveAs" does not work. In these cases, it is recommended to use "SaveCopyAs"
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveCopyAs(string fileName)
        {
            _app.DisplayAlerts = false;
            _workBook.Saved = true;
            _workBook.SaveCopyAs(fileName);
        }

        /// <summary>
        /// Adding a new sheet to the document
        /// </summary>
        /// <param name="nameSheet">The size must not exceed 32 characters</param>
        /// <returns></returns>
        public ISheetFastExcel AddNewSheet(string nameSheet)
        {
            var workSheet = (Worksheet)_workBook.Worksheets.Add(System.Type.Missing, _app.Worksheets[_app.Worksheets.Count], 1, XlSheetType.xlWorksheet);
            workSheet.Name = nameSheet;

            return _sheetFactory.Get(workSheet);
        }

        public void Dispose()
        {
            try
            {
                if (_app != null && _app.Visible != true)
                {
                    _app.DisplayAlerts = false;
                    _app.Quit();
                }
            }
            catch
            {
                
            }
        }
    }
}
