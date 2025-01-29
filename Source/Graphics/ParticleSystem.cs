
namespace Celeste64;

public readonly struct ParticleTheme
{
	public readonly float Rate { get; init; }
	public readonly string Sprite { get; init; }
	public readonly Vec3 Gravity { get; init; }
	public readonly Vec3 StartVelocity { get; init; }
	public readonly float Life { get; init; }
	public readonly float Size { get; init; }
}

public class ParticleSystem
{
	private struct Particle
	{
		public Vec3 Position;
		public float Life;
		public Vec3 Velocity;
	}

	public float Accumulator = 0;
	public ParticleTheme Theme;
	public float MaximumDistance = 400;
	public readonly int MaxParticles;

	private readonly List<Particle> particles;
	private float deltaTime;

	public ParticleSystem(int maxParticles, in ParticleTheme theme)
	{
		particles = new(MaxParticles = maxParticles);
		Theme = theme;
	}

	public void SpawnParticle(Vec3 position, Vec3 velocity, float rateMultiplier)
	{
		Accumulator += rateMultiplier * Theme.Rate * deltaTime;

		while (Accumulator > 0)
		{
			if (particles.Count >= MaxParticles)
				particles.RemoveAt(0);
				
			particles.Add(new Particle() with
			{
				Position = position,
				Velocity = Theme.StartVelocity + velocity,
				Life = Theme.Life
			});

			Accumulator--;
		}
	}
		
	public void Update(float deltaTime)
	{
		this.deltaTime = deltaTime;

		for (int i = particles.Count - 1; i >= 0; i--)
		{
			var it = particles[i];

			it.Life -= deltaTime;
			it.Velocity += Theme.Gravity * deltaTime;
			it.Position += it.Velocity * deltaTime;

			if (it.Life <= 0)
				particles.RemoveAt(i);
			else	
				particles[i] = it;
		}
	}

	public void CollectSprites(Vec3 source, World world, List<Sprite> populate)
	{
		if ((world.Camera.Position - source).LengthSquared() > MaximumDistance * MaximumDistance)
			return;

		for (int i = particles.Count - 1; i >= 0; i--)
		{
			if (particles[i].Life <= 0)
				continue;
				
			populate.Add(Sprite.CreateBillboard(world, particles[i].Position, Theme.Sprite, Theme.Size * particles[i].Life / Theme.Life, Color.White));
		}
	}
}