import { stores } from "../data/landingData";
import { StoreCard } from "./StoreCard";

export function StoresSection() {
  return (
    <section id="stores" className="border-t border-line bg-paper py-20 md:min-h-[55vh] md:py-[100px]">
      <div className="container-page">
        <h2 className="font-serif text-[clamp(48px,6vw,80px)] font-normal leading-[0.95] tracking-[-0.055em] text-ink">
          Our Stores
        </h2>
        <div className="mt-14 grid gap-12 md:grid-cols-2 md:gap-16">
          {stores.map((store) => (
            <StoreCard key={store.id} store={store} />
          ))}
        </div>
      </div>
    </section>
  );
}
