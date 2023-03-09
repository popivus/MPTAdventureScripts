using UnityEngine;

public class Config
{
    public Config(KeyCode interact, KeyCode drop, KeyCode shoot, KeyCode run)
    {
        this.interact = interact;
        this.drop = drop;
        this.shoot = shoot;
        this.run = run;
    }

    public Config()
    {
    }

    public KeyCode interact {get;set;}
    public KeyCode drop {get;set;}
    public KeyCode shoot {get;set;}
    public KeyCode run {get;set;}
}