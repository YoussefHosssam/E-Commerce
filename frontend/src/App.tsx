import { useEffect, useMemo, useState } from "react";
import { AboutSection } from "./components/AboutSection";
import { CartDrawer } from "./components/CartDrawer";
import { CheckoutPage } from "./components/CheckoutPage";
import { FeaturedProducts } from "./components/FeaturedProducts";
import { Footer } from "./components/Footer";
import { Header } from "./components/Header";
import { Hero } from "./components/Hero";
import { MegaMenu } from "./components/MegaMenu";
import { ProductDetailPage } from "./components/ProductDetailPage";
import { ProductModal } from "./components/ProductModal";
import { RoomsSection } from "./components/RoomsSection";
import { RoomsPage } from "./components/RoomsPage";
import { ShopPage } from "./components/ShopPage";
import { SolveCollageSection } from "./components/SolveCollageSection";
import { StoresSection } from "./components/StoresSection";
import { shopProducts } from "./data/landingData";
import type { CartItem, Product } from "./types/commerce";

function App() {
  const [pathname, setPathname] = useState(() => window.location.pathname);
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isCartOpen, setIsCartOpen] = useState(false);
  const [isFooterVisible, setIsFooterVisible] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [cartItems, setCartItems] = useState<CartItem[]>([]);

  const isShopPage = pathname === "/shop";
  const isCheckoutPage = pathname === "/checkout";
  const isRoomsRoute = pathname === "/rooms" || pathname.startsWith("/rooms/");
  const roomSlug = pathname.startsWith("/rooms/") ? decodeURIComponent(pathname.replace("/rooms/", "")) : undefined;
  const productSlug = pathname.startsWith("/shop/") ? decodeURIComponent(pathname.replace("/shop/", "")) : "";
  const currentProduct = productSlug ? shopProducts.find((product) => product.slug === productSlug) : undefined;
  const isShopRoute = pathname === "/shop" || Boolean(productSlug);
  const usesDefaultHeader = isShopRoute || isCheckoutPage;

  useEffect(() => {
    const syncPathname = () => setPathname(window.location.pathname);

    window.addEventListener("popstate", syncPathname);
    return () => window.removeEventListener("popstate", syncPathname);
  }, []);

  useEffect(() => {
    if (!isRoomsRoute) {
      setIsFooterVisible(false);
      return;
    }

    const footer = document.getElementById("site-footer");

    if (!footer) {
      return;
    }

    const observer = new IntersectionObserver(
      ([entry]) => setIsFooterVisible(entry.isIntersecting),
      { threshold: 0.15 },
    );

    observer.observe(footer);
    return () => observer.disconnect();
  }, [isRoomsRoute]);

  const cartCount = useMemo(
    () => cartItems.reduce((total, item) => total + item.quantity, 0),
    [cartItems],
  );

  const navigate = (to: string) => {
    window.history.pushState({}, "", to);
    setPathname(window.location.pathname);
    setIsMenuOpen(false);
    window.scrollTo({ top: 0, behavior: "instant" });
  };

  const navigateToProduct = (product: Product) => {
    navigate(`/shop/${product.slug}`);
  };

  const navigateToShopFromCart = () => {
    setIsCartOpen(false);
    navigate("/shop");
  };

  const navigateToCheckoutFromCart = () => {
    setIsCartOpen(false);
    navigate("/checkout");
  };

  const addToCart = (product: Product, quantity: number) => {
    setCartItems((items) => {
      const existingItem = items.find((item) => item.product.id === product.id);

      if (existingItem) {
        return items.map((item) =>
          item.product.id === product.id ? { ...item, quantity: item.quantity + quantity } : item,
        );
      }

      return [...items, { product, quantity }];
    });
    setIsCartOpen(true);
  };

  const removeFromCart = (productId: string) => {
    setCartItems((items) => items.filter((item) => item.product.id !== productId));
  };

  const setQuantity = (productId: string, quantity: number) => {
    const nextQuantity = Math.max(1, quantity);

    setCartItems((items) =>
      items.map((item) =>
        item.product.id === productId ? { ...item, quantity: nextQuantity } : item,
      ),
    );
  };

  const increaseQuantity = (productId: string) => {
    setCartItems((items) =>
      items.map((item) =>
        item.product.id === productId ? { ...item, quantity: item.quantity + 1 } : item,
      ),
    );
  };

  const decreaseQuantity = (productId: string) => {
    setCartItems((items) =>
      items.map((item) =>
        item.product.id === productId ? { ...item, quantity: Math.max(1, item.quantity - 1) } : item,
      ),
    );
  };

  return (
    <div className="min-h-screen bg-paper text-ink">
      <Header
        cartCount={cartCount}
        onCartOpen={() => setIsCartOpen(true)}
        onMenuOpen={() => setIsMenuOpen((value) => !value)}
        isMenuOpen={isMenuOpen}
        tone={isRoomsRoute && !isFooterVisible ? "inverse" : usesDefaultHeader ? "default" : "inverse"}
        homeHref={usesDefaultHeader ? "/" : "#top"}
        className={isRoomsRoute ? "rooms-fixed-header" : ""}
      />
      {productSlug ? (
        <ProductDetailPage
          product={currentProduct}
          onAddToCart={addToCart}
          onProductNavigate={navigateToProduct}
        />
      ) : isRoomsRoute ? (
        <RoomsPage
          initialSlug={roomSlug}
          onExploreRoom={(slug) => navigate(`/rooms/${slug}`)}
        />
      ) : isCheckoutPage ? (
        <CheckoutPage
          items={cartItems}
          onProductNavigate={navigateToProduct}
          onShopNavigate={() => navigate("/shop")}
        />
      ) : isShopPage ? (
        <ShopPage onProductNavigate={navigateToProduct} />
      ) : (
        <main>
          <Hero />
          <FeaturedProducts onProductSelect={setSelectedProduct} />
          <AboutSection />
          <SolveCollageSection />
          <RoomsSection onSeeAll={() => navigate("/rooms")} />
          <StoresSection />
        </main>
      )}
      <Footer />
      <MegaMenu
        isOpen={isMenuOpen}
        onClose={() => setIsMenuOpen(false)}
      />
      <CartDrawer
        isOpen={isCartOpen}
        items={cartItems}
        onClose={() => setIsCartOpen(false)}
        onRemove={removeFromCart}
        onIncreaseQuantity={increaseQuantity}
        onDecreaseQuantity={decreaseQuantity}
        onSetQuantity={setQuantity}
        onShopNow={navigateToShopFromCart}
        onCheckout={navigateToCheckoutFromCart}
      />
      <ProductModal product={selectedProduct} onClose={() => setSelectedProduct(null)} onAddToCart={addToCart} />
    </div>
  );
}

export default App;
