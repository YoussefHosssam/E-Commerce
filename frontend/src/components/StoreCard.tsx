import type { Store } from "../types/commerce";

type StoreCardProps = {
  store: Store;
};

export function StoreCard({ store }: StoreCardProps) {
  return (
    <article className="flex flex-col items-start gap-7 border-t border-line pt-8 sm:flex-row sm:items-center">
      <img className="size-[140px] rounded-full object-cover" src={store.image} alt={`${store.city} store`} />
      <div>
        <h3 className="font-serif text-[42px] font-normal leading-none tracking-[-0.055em] text-ink">{store.city}</h3>
        <address className="mt-5 not-italic text-[13px] uppercase leading-[1.7] tracking-[0.08em] text-muted">
          {store.address.map((line) => (
            <span key={line} className="block">
              {line}
            </span>
          ))}
        </address>
        <a className="eyebrow-link mt-7 inline-flex text-ink" href={`https://maps.google.com/?q=${store.city}`}>
          Get Directions &rarr;
        </a>
      </div>
    </article>
  );
}
