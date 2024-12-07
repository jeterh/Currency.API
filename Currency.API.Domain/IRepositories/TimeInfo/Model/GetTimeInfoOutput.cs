﻿
namespace Currency.API.Domain.IRepositories.TimeInfo.Model
{
	public class GetTimeInfoOutput
	{
        public Guid Id { get; set; }
        public string Updated { get; set; } = null!;    
        public string UpdatedISO { get;} = null!;
		public string UpdatedUK { get; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
