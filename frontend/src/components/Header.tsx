import { navLinks } from "../data/landingData";

type HeaderProps = {
  cartCount: number;
  onCartOpen: () => void;
  onMenuOpen: () => void;
  isMenuOpen?: boolean;
  tone?: "inverse" | "default";
  homeHref?: string;
  className?: string;
};

export function Header({
  cartCount,
  onCartOpen,
  onMenuOpen,
  isMenuOpen = false,
  tone = "inverse",
  homeHref = "#top",
  className = "",
}: HeaderProps) {
  const isInverse = tone === "inverse";
  const textColor = isInverse ? "text-white" : "text-ink";
  const borderColor = isInverse ? "border-white/65" : "border-ink/35";
  const cartTone = isInverse ? "bg-white text-ink" : "bg-ink text-white";
  const logoSize = isInverse ? "md:text-[26px]" : "md:text-[24px]";
  const navSize = isInverse ? "" : "shop-header-link";

  return (
    <header className={`absolute inset-x-0 top-0 z-[100] ${textColor} ${className}`}>
      <div className="site-header-grid">
        <a className={`header-nav-link ${navSize} hidden justify-self-start sm:inline-flex ${textColor}`} href={navLinks[0].href}>
          {navLinks[0].label}
        </a>
        <a
          className={`justify-self-center font-serif text-[22px] font-normal uppercase leading-none tracking-[-0.06em] ${logoSize} ${textColor}`}
          href={homeHref}
          aria-label="SOLVE home"
        >
          SOLVE
        </a>
        <nav className="flex items-center justify-end gap-5 md:gap-7">
          <a className={`header-nav-link ${navSize} hidden md:inline-flex ${textColor}`} href={navLinks[1].href}>
            {navLinks[1].label}
          </a>
          <button className={`header-nav-link ${navSize} inline-flex items-center ${textColor}`} type="button" onClick={onCartOpen}>
            <span>Cart</span>
            <span className={`cart-count ${cartTone}`}>{cartCount}</span>
          </button>
          <button
            className={`grid place-items-center transition-colors duration-300 ${
              isMenuOpen ? `size-12 rounded-full border ${borderColor}` : "h-12 w-12"
            }`}
            type="button"
            onClick={onMenuOpen}
            aria-label={isMenuOpen ? "Close menu" : "Open menu"}
            aria-expanded={isMenuOpen}
          >
            <span className={`hamburger-lines ${isInverse ? "[--hamburger-color:#FFFFFF]" : "[--hamburger-color:#343339]"}`} aria-hidden="true">
              <span />
              <span />
            </span>
          </button>
        </nav>
      </div>
    </header>
  );
}
