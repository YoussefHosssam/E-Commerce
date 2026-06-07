import type { Product } from "../types/commerce";

type ProductCardProps = {
  product: Product;
  onSelect: (product: Product) => void;
};

export function ProductCard({ product, onSelect }: ProductCardProps) {
  return (
    <article className="group text-center">
      <button
        className="relative mx-auto block overflow-visible"
        type="button"
        onClick={() => onSelect(product)}
      >
        <span className="block h-[380px] w-[280px] overflow-hidden rounded-[999px] md:h-[clamp(320px,30vw,460px)] md:w-[clamp(230px,22vw,330px)]">
          <img
            className="block h-full w-full object-cover transition-transform duration-700 group-hover:scale-[1.035]"
            src={product.image}
            alt={product.name}
          />
        </span>
        {product.badge ? (
          <span className="absolute right-[-28px] top-6 flex size-16 items-center justify-center rounded-full bg-[#343339] text-[10px] font-bold uppercase tracking-[0.05em] text-white">
            {product.badge}
          </span>
        ) : null}
      </button>
      <button className="product-meta mt-[34px] block w-full" type="button" onClick={() => onSelect(product)}>
        <span className="block text-[11px] font-semibold leading-none text-ink">{product.name}</span>
        <span className="mt-2 block text-[10px] font-semibold leading-none text-ink">{product.price}</span>
      </button>
    </article>
  );
}
