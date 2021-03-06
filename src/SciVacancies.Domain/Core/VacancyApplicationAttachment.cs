﻿using System;

namespace SciVacancies.Domain.Core
{
    public class VacancyApplicationAttachment
    {
        /// <summary>
        /// Guid прикреплённого файла
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Наименование файла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Тип файла (резюме, портфолио и т.д.)
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Дата загрузки файла на сервер
        /// </summary>
        public DateTime UploadDate { get; set; }
    }
}
