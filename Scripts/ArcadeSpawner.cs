using Godot;
using System;
using Godot.Collections;

public partial class ArcadeSpawner : Node3D {

    private PackedScene healthpotion;
    public static ArcadeSpawner Instance { get; private set; }
    public Array<CharacterBody3D> enemies = new Array<CharacterBody3D>();
    private Array<Marker3D> spawnMarkers;
    private Array<int> enemiesToSpawn = new Array<int>();

    private const int BASE_TOKENS = 20;
    private const int TIME_BETWEEN_WAVES = 30;
    private const int DEATH_PER_POTION = 10; 
    private uint NUMBER_OF_AVAILABLE_MONSTERS = 2;

    private int currentTokens = BASE_TOKENS;
    private float timeSinceLastWave = TIME_BETWEEN_WAVES;
    private float timeBetweenSpawns = 0;
    private float timeBetweenSpawnsTick = 0;
    public int currentWave = 0;
    private int currentDeaths = 0; 

    public bool mobsShouldSpawn = false;

    // Max vill ha en stat class for varje enemy sa att man inte behover skapa nya baseStats for varje ny instans det enda som behover vara privat i
    // en fiende ar deras egna nuvarande liv
    public override void _Ready() {
        Instance = this;
        healthpotion = GD.Load<PackedScene>("res://Scenes/health_potion.tscn"); 
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        timeSinceLastWave += (float)delta;
        //debug option F4----
        if (Input.IsActionJustPressed("DespawnMobs") && PlayerManager.Instance.debugBoolean) {
            var enemies = GetTree().GetNodesInGroup("enemies");
            foreach (var enemy in enemies) {
                ((CharacterBody3D)enemy).QueueFree();
            }
            this.enemies.Clear();
        }
        //debug option F2
        if (Input.IsActionJustPressed("SpawnMobs") && PlayerManager.Instance.debugBoolean) {
            mobsShouldSpawn = !mobsShouldSpawn;
        }

        if (mobsShouldSpawn) {
            if (timeSinceLastWave >= TIME_BETWEEN_WAVES) {
                CreateWave();
                timeSinceLastWave = 0;
            }
            SpawnEnemies(delta);
        }
    }

    private void CreateWave() {
        currentWave++;
        while (currentTokens > 0) {
            uint chosenEnemyNumber = GD.Randi() % NUMBER_OF_AVAILABLE_MONSTERS;

            switch (chosenEnemyNumber) {
                case 0:
                    if (currentTokens - 1 >= 0) {
                        currentTokens -= 1;
                        enemiesToSpawn.Add(1);
                    }
                    break;
                case 1:
                    if (currentTokens - 2 >= 0) {
                        currentTokens -= 2;
                        enemiesToSpawn.Add(2);
                    }
                    break;
                case 2:
                    if (currentTokens - 5 >= 0) {
                        currentTokens -= 5;
                        enemiesToSpawn.Add(3);
                    }
                    break;
                default:
                    GD.PushWarning("chosenEnemyNumber does not exist");
                    break;
            }
        }
        timeBetweenSpawns = (TIME_BETWEEN_WAVES / 2) / (float)enemiesToSpawn.Count;
        if (currentWave == 4) {
            NUMBER_OF_AVAILABLE_MONSTERS = 3;
        }
        currentTokens = (int)(BASE_TOKENS * (float)(Math.Pow(1.1f, currentWave) + currentWave/5f));
        GD.Print(currentWave + " Wave | " + currentTokens + " Tokens");
    }

    private void SpawnEnemies(double delta) {
        timeBetweenSpawnsTick += (float)delta;
        while (timeBetweenSpawnsTick > timeBetweenSpawns || enemies.Count < 5) {
            timeBetweenSpawnsTick -= timeBetweenSpawns;
            if (enemiesToSpawn.Count == 0) return;

            Array<Node> enemySpawnPoints = GetTree().GetNodesInGroup("EnemySpawnPoints");
            uint chosenSpawnPoint = GD.Randi() % (uint)enemySpawnPoints.Count;

            Node enemy = null;
            switch (enemiesToSpawn[enemiesToSpawn.Count - 1]) {
                case 1:
                    enemy = GameManager.Instance.skeleton.Instantiate();
                    break;
                case 2:
                    enemy = GameManager.Instance.rogue.Instantiate();
                    break;
                case 3:
                    enemy = GameManager.Instance.mage.Instantiate();
                    break;
            }
            PlayerManager.Instance.AddChild(enemy);

            //float dirAngle = GD.Randf() * (2 * MathF.PI);
            //Transform3D lookTransfrom = new Transform3D(new Basis(Transform.Basis.Y, dirAngle), new Vector3(0, 2, 0));

            //(enemy as CharacterBody3D).Transform = GetParentNode3D().Transform;
            //(enemy as CharacterBody3D).Position += lookTransfrom.Basis.Y;
            //(enemy as CharacterBody3D).Position += -lookTransfrom.Basis.Z * 17;

            (enemy as CharacterBody3D).GlobalPosition = (enemySpawnPoints[(int)chosenSpawnPoint] as Marker3D).GlobalPosition;

            enemiesToSpawn.RemoveAt(enemiesToSpawn.Count - 1);
        }
    }
    public void TrySpawnPotion(Vector3 position)
    {
        currentDeaths++;
        if(currentDeaths>=DEATH_PER_POTION)
        {
            var instance = healthpotion.Instantiate();
            GetParent().GetParent().AddChild(instance); 
            (instance as Node3D).GlobalPosition = position; 
            currentDeaths = 0; 


        }

    }
}
