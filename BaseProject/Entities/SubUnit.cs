﻿namespace BaseProject.Entities;

public class SubUnit : BaseEntities
{
    public string? SubName { get; set; }
    public IList<Employee> Employees { get; set; }

    public SubUnit()
    {
        Employees = [];
    }
}