import { heroImage } from "../data/landingData";

export function Hero() {
  const scrollToFeatured = () => {
    document.getElementById("featured")?.scrollIntoView({ behavior: "smooth" });
  };

  return (
    <section
      id="top"
      className="relative h-screen min-h-screen overflow-hidden md:min-h-[720px]"
      aria-label="Design furniture"
    >
      <img
        className="absolute inset-0 h-full w-full object-cover object-center"
        src={heroImage}
        alt="Yellow and gray modern chairs"
      />
      <div className="absolute inset-0 bg-[rgba(20,16,14,0.45)]" />
      <div className="container-page relative z-10 flex h-full flex-col items-center justify-center pt-[90px] text-center text-white">
        <h1 className="font-serif text-[clamp(64px,18vw,100px)] font-normal uppercase leading-[0.82] tracking-[-0.06em] text-white md:text-[clamp(90px,12vw,190px)]">
          <span className="block">Design</span>
          <span className="block">Furniture</span>
        </h1>
        <button
          className="tiny-button mt-9 rounded-full border border-white/75 px-7 py-[14px] text-white transition-all duration-300 hover:bg-white hover:text-ink"
          type="button"
          onClick={scrollToFeatured}
        >
          Shop Now
        </button>
      </div>
    </section>
  );
}
