import { Minus, Plus, X } from "lucide-react";
import { useState } from "react";
import type { Product } from "../types/commerce";

type ProductModalProps = {
  product: Product | null;
  onClose: () => void;
  onAddToCart: (product: Product, quantity: number) => void;
};

export function ProductModal({ product, onClose, onAddToCart }: ProductModalProps) {
  const [quantity, setQuantity] = useState(1);
  const isOpen = Boolean(product);

  if (!product) {
    return null;
  }

  const addToCart = () => {
    onAddToCart(product, quantity);
    setQuantity(1);
    onClose();
  };

  return (
    <div className={isOpen ? "pointer-events-auto" : "pointer-events-none"}>
      <div className="panel-backdrop" onClick={onClose} />
      <section
        className="fixed left-1/2 top-1/2 z-50 grid max-h-[90vh] w-[calc(100%-40px)] max-w-[900px] -translate-x-1/2 -translate-y-1/2 overflow-y-auto bg-paper shadow-drawer md:grid-cols-[0.9fr_1fr]"
        aria-label={`${product.name} details`}
      >
        <div className="h-[360px] overflow-hidden md:h-full">
          <img className="h-full w-full object-cover" src={product.image} alt={product.name} />
        </div>
        <div className="relative p-8 md:p-12">
          <button
            className="absolute right-6 top-6 grid size-10 place-items-center rounded-full border border-line transition-colors duration-300 hover:bg-white"
            type="button"
            onClick={onClose}
            aria-label="Close product details"
          >
            <X size={17} strokeWidth={1.6} />
          </button>
          <p className="eyebrow-link text-muted">Featured</p>
          <h2 className="mt-8 font-serif text-[58px] font-normal leading-[0.92] tracking-[-0.055em] text-ink">{product.name}</h2>
          <p className="mt-5 text-[11px] font-medium uppercase tracking-[0.08em] text-muted">{product.price}</p>
          <p className="mt-8 text-[14px] leading-[1.9] text-muted">{product.description}</p>
          <div className="mt-10 flex items-center gap-5">
            <div className="flex h-12 items-center rounded-full border border-line">
              <button
                className="grid size-12 place-items-center"
                type="button"
                onClick={() => setQuantity((value) => Math.max(1, value - 1))}
                aria-label="Decrease quantity"
              >
                <Minus size={14} strokeWidth={1.6} />
              </button>
              <span className="min-w-8 text-center text-[12px] font-medium">{quantity}</span>
              <button
                className="grid size-12 place-items-center"
                type="button"
                onClick={() => setQuantity((value) => value + 1)}
                aria-label="Increase quantity"
              >
                <Plus size={14} strokeWidth={1.6} />
              </button>
            </div>
            <button
              className="tiny-button h-12 rounded-full bg-ink px-8 text-white transition-opacity duration-300 hover:opacity-80"
              type="button"
              onClick={addToCart}
            >
              Add to cart
            </button>
          </div>
        </div>
      </section>
    </div>
  );
}
