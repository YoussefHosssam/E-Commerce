import { useEffect, useState } from "react";
import { products } from "../data/landingData";
import type { Product } from "../types/commerce";

type ProductDetailPageProps = {
  product: Product | undefined;
  onAddToCart: (product: Product, quantity: number) => void;
  onProductNavigate: (product: Product) => void;
};

const detailsText =
  "Mauris cursus mattis molestie a iaculis at erat pellentesque adipiscing. Netus et malesuada fames ac turpis egestas integer eget. A diam maecenas sed enim ut sem viverra aliquet eget. Vel fringilla est ullamcorper eget nulla facilisi etiam. Velit egestas dui id ornare arcu odio ut. Felis donec et odio pellentesque diam volutpat commodo sed egestas.";

const introText =
  "Mauris cursus mattis molestie a iaculis at erat pellentesque adipiscing. Netus et malesuada fames ac turpis egestas integer eget.";

const deliveryText =
  "Mauris cursus mattis molestie a iaculis at erat pellentesque adipiscing. Netus et malesuada fames ac turpis egestas integer eget. A diam maecenas sed enim ut sem viverra aliquet eget. Vel fringilla est ullamcorper eget nulla facilisi etiam.";

const returnsText =
  "Mauris cursus mattis molestie a iaculis at erat pellentesque adipiscing. Netus et malesuada fames ac turpis egestas integer eget.";

const detailBullets = [
  "Proin fermentum leo vel orci porta non pulvinar",
  "Diam phasellus vestibulum",
  "Quisque egestas diam in arcu cursus",
];

function ProductTitle({ name }: { name: string }) {
  const words = name.split(" ");

  return (
    <>
      {words.map((word) => (
        <span key={word} className="block">
          {word}
        </span>
      ))}
    </>
  );
}

export function ProductDetailPage({
  product,
  onAddToCart,
  onProductNavigate,
}: ProductDetailPageProps) {
  const [isDetailsOpen, setIsDetailsOpen] = useState(false);
  const [quantity, setQuantity] = useState(1);

  const openDetailsDrawer = () => {
    setIsDetailsOpen(true);
  };

  useEffect(() => {
    setQuantity(1);
  }, [product?.id]);

  if (!product) {
    return (
      <main className="product-not-found">
        <h1>Not Found</h1>
        <p>The product you are looking for is not available.</p>
        <a href="/shop">Back to shop</a>
      </main>
    );
  }

  return (
    <main id="top" className="bg-paper text-ink">
      <section className="product-single-hero" aria-labelledby="product-title">
        <div className="product-image-panel">
          <img src={product.image} alt={product.name} />
        </div>

        <div className="product-info-panel">
          <div className="product-info-content">
            <h1 id="product-title" className="product-single-title">
              <ProductTitle name={product.name} />
            </h1>

            <p className="product-single-price">{product.price}</p>
            <p className="product-single-intro">{introText}</p>

            <div className="product-cart-row">
              <div className="quantity-stepper" aria-label="Product quantity">
                <button
                  type="button"
                  onClick={() => setQuantity((value) => Math.max(1, value - 1))}
                  aria-label="Decrease quantity"
                >
                  -
                </button>
                <span>{quantity}</span>
                <button
                  type="button"
                  onClick={() => setQuantity((value) => value + 1)}
                  aria-label="Increase quantity"
                >
                  +
                </button>
              </div>
              <button
                className="product-add-button"
                type="button"
                onClick={() => onAddToCart(product, quantity)}
              >
                Add to cart
              </button>
            </div>

            <div className="product-details-row">
              <button type="button" onClick={openDetailsDrawer}>
                Details
              </button>
              <button type="button" onClick={openDetailsDrawer}>
                Delivery
              </button>
              <button type="button" onClick={openDetailsDrawer}>
                Returns
              </button>
            </div>
          </div>
        </div>
      </section>

      <section className="product-description-section" aria-label="Description">
        <div className="product-description-grid">
          <h2>Description</h2>
          <div>
            <p>{detailsText}</p>
            <ul>
              {detailBullets.map((item) => (
                <li key={item}>{item}</li>
              ))}
            </ul>
          </div>
        </div>
      </section>

      <section className="product-related-section" aria-labelledby="related-title">
        <h2 id="related-title">Related</h2>

        <div className="product-related-grid">
          {products.map((relatedProduct) => (
            <article key={relatedProduct.id} className="product-related-card">
              <button type="button" onClick={() => onProductNavigate(relatedProduct)}>
                <span className="product-related-image">
                  <img src={relatedProduct.image} alt={relatedProduct.name} />
                </span>

                {relatedProduct.badge ? (
                  <span className="product-related-badge">{relatedProduct.badge}</span>
                ) : null}
              </button>

              <button
                className="product-related-info"
                type="button"
                onClick={() => onProductNavigate(relatedProduct)}
              >
                <span>{relatedProduct.name}</span>
                <span>{relatedProduct.price}</span>
              </button>
            </article>
          ))}
        </div>
      </section>

      {isDetailsOpen ? (
        <div
          className="details-overlay"
          role="dialog"
          aria-modal="true"
          aria-label={`${product.name} details`}
        >
          <div className="details-content-panel">
            <div className="details-content-inner">
              <button
                className="close-details"
                type="button"
                onClick={() => setIsDetailsOpen(false)}
              >
                Close
              </button>

              <section>
                <h2>Details</h2>
                <p>{detailsText}</p>

                <ul>
                  {detailBullets.map((item) => (
                    <li key={item}>{item}</li>
                  ))}
                </ul>
              </section>

              <section>
                <h2>Delivery</h2>
                <p>{deliveryText}</p>
              </section>

              <section>
                <h2>Returns</h2>
                <p>{returnsText}</p>
              </section>
            </div>
          </div>
        </div>
      ) : null}
    </main>
  );
}
