import { useEffect, useMemo, useState } from "react";
import { shopProducts } from "../data/landingData";
import type { Product, ProductCategory } from "../types/commerce";

type ShopPageProps = {
  onProductNavigate: (product: Product) => void;
};

const categories: ProductCategory[] = ["sofa", "lamp", "chair"];
const productsPerPage = 6;

function getCategoryFromUrl() {
  const value = new URLSearchParams(window.location.search).get("category");
  return categories.includes(value as ProductCategory) ? (value as ProductCategory) : null;
}

export function ShopPage({ onProductNavigate }: ShopPageProps) {
  const [activeCategory, setActiveCategory] = useState<ProductCategory | null>(() => getCategoryFromUrl());
  const [page, setPage] = useState(1);

  useEffect(() => {
    const syncCategory = () => {
      setActiveCategory(getCategoryFromUrl());
      setPage(1);
    };

    window.addEventListener("popstate", syncCategory);
    return () => window.removeEventListener("popstate", syncCategory);
  }, []);

  const filteredProducts = useMemo(
    () =>
      activeCategory
        ? shopProducts.filter((product) => product.category === activeCategory)
        : shopProducts,
    [activeCategory],
  );

  const totalPages = Math.max(1, Math.ceil(filteredProducts.length / productsPerPage));
  const currentPage = Math.min(page, totalPages);
  const visibleProducts = filteredProducts.slice(
    (currentPage - 1) * productsPerPage,
    currentPage * productsPerPage,
  );

  const selectCategory = (category: ProductCategory) => {
    const nextCategory = activeCategory === category ? null : category;
    const nextUrl = nextCategory ? `/shop?category=${nextCategory}` : "/shop";

    window.history.pushState({}, "", nextUrl);
    setActiveCategory(nextCategory);
    setPage(1);
  };

  return (
    <main id="top" className="bg-paper text-ink">
      <section className="shop-title-section" aria-labelledby="shop-title">
        <h1 id="shop-title" className="shop-title">
          SHOP
        </h1>
      </section>

      <nav className="shop-category-bar" aria-label="Product categories">
        {categories.map((category) => (
          <button
            key={category}
            className={`shop-category-button ${activeCategory === category ? "is-active" : ""}`}
            type="button"
            onClick={() => selectCategory(category)}
          >
            {category}
          </button>
        ))}
      </nav>

      <section className="shop-products-section" aria-label="Products">
        <div className="shop-product-grid">
          {visibleProducts.map((product) => (
            <article key={product.id} className="shop-product">
              <button
                className="shop-product-media"
                type="button"
                onClick={() => onProductNavigate(product)}
              >
                <img src={product.image} alt={product.name} />
                {product.badge ? <span className="shop-product-badge">{product.badge}</span> : null}
              </button>
              <button
                className="shop-product-info"
                type="button"
                onClick={() => onProductNavigate(product)}
              >
                <span className="shop-product-name">{product.name}</span>
                <span className="shop-product-price">{product.price}</span>
              </button>
            </article>
          ))}
        </div>
      </section>

      <nav className="shop-pagination" aria-label="Product pagination">
        <button
          className="shop-page-button shop-page-pill"
          type="button"
          disabled={currentPage === 1}
          onClick={() => setPage((value) => Math.max(1, value - 1))}
        >
          Prev
        </button>
        {Array.from({ length: totalPages }, (_, index) => index + 1).map((pageNumber) => (
          <button
            key={pageNumber}
            className={`shop-page-button ${currentPage === pageNumber ? "is-active" : ""}`}
            type="button"
            onClick={() => setPage(pageNumber)}
          >
            {pageNumber}
          </button>
        ))}
        <button
          className="shop-page-button shop-page-pill"
          type="button"
          disabled={currentPage === totalPages}
          onClick={() => setPage((value) => Math.min(totalPages, value + 1))}
        >
          Next
        </button>
      </nav>

      <section className="shop-help-section" aria-label="Help">
        <div className="shop-help-grid">
          <h2>Help?</h2>
          <article>
            <h3>Local Stores</h3>
            <p>
              Proin fermentum leo vel orci porta non pulvinar. Diam phasellus vestibulum lorem sed risus ultricies.
            </p>
            <a href="#stores">Find a store <span aria-hidden="true">-&gt;</span></a>
          </article>
          <article>
            <h3>Questions?</h3>
            <p>
              Proin fermentum leo vel orci porta non pulvinar. Diam phasellus vestibulum lorem sed risus ultricies.
            </p>
            <a href="#faq">Read the FAQ <span aria-hidden="true">-&gt;</span></a>
          </article>
        </div>
      </section>
    </main>
  );
}
