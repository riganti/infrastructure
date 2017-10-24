using AutoMapper;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public static class MappingHelper
    {
        private static bool isConfigured;
        private static readonly object syncRoot = new object();

        public static void Config()
        {
            if (!isConfigured)
            {
                lock (syncRoot)
                {
                    if (!isConfigured)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<EmployeeDTO, Employee>();
                            cfg.CreateMap<Employee, EmployeeDTO>();
                            cfg.CreateMap<EmployeeProjectDTO, EmployeeProject>();
                            cfg.CreateMap<EmployeeProject, EmployeeProjectDTO>();
                        });

                        isConfigured = true;
                    }
                }
            }
        }
    }
}