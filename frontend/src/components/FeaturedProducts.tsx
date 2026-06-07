import { products } from "../data/landingData";
import type { Product } from "../types/commerce";
import { ProductCard } from "./ProductCard";

type FeaturedProductsProps = {
  onProductSelect: (product: Product) => void;
};

export function FeaturedProducts({ onProductSelect }: FeaturedProductsProps) {
  return (
    <section id="featured" className="bg-paper py-[70px] md:min-h-screen md:pb-[120px] md:pt-[110px]">
      <div className="mx-auto w-full max-w-[1120px] px-5 md:px-12">
        <h2 className="mb-14 text-center font-serif text-[clamp(72px,8vw,116px)] font-normal leading-[0.9] tracking-[-0.055em] text-ink md:mb-20">
          Featured
        </h2>
        <div className="grid justify-items-center gap-[60px] md:grid-cols-3 md:gap-[72px]">
          {products.map((product) => (
            <ProductCard key={product.id} product={product} onSelect={onProductSelect} />
          ))}
        </div>
      </div>
    </section>
  );
}
