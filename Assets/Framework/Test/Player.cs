using System.Collections;
using System.Collections.Generic;
using Framework.DataStruct;
using Framework.SQLite3;
using UnityEngine;

public enum PlayerEnum
{
    ID,
    Name,
    Max
}
public class Player : Base
{
    [Constraint(SQLite3Constraint.PrimaryKey | SQLite3Constraint.AutoIncrement)]
    [Sync((int)PlayerEnum.ID)]
    public int ID { get; private set; }

    [Constraint(SQLite3Constraint.NotNull)]
    [Sync((int)PlayerEnum.Name)]
    public string Name { get; private set; }
}

