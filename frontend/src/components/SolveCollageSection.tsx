import { collageBackgroundImage } from "../data/landingData";

export function SolveCollageSection() {
  return (
    <section
      className="relative h-[460px] overflow-hidden bg-cover bg-center md:h-[75vh] md:min-h-[560px]"
      style={{ backgroundImage: `url(${collageBackgroundImage})` }}
      aria-label="SOLVE"
    >
      <div className="absolute inset-0 bg-[rgba(20,16,14,0.18)]" />
      <h2 className="absolute left-1/2 top-1/2 z-10 -translate-x-1/2 -translate-y-1/2 font-serif text-[clamp(90px,14vw,190px)] font-normal uppercase leading-none tracking-[-0.07em] text-white">
        SOLVE
      </h2>
    </section>
  );
}
