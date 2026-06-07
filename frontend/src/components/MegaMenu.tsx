import { menuGroups, products } from "../data/landingData";

type MegaMenuProps = {
  isOpen: boolean;
  onClose: () => void;
};

export function MegaMenu({ isOpen, onClose }: MegaMenuProps) {
  const getMenuHref = (link: string) => {
    if (link === "Home") {
      return "/";
    }

    if (link === "Rooms") {
      return "/rooms";
    }

    if (link === "Lookbook") {
      return "#lookbook";
    }

    return `#${link.toLowerCase()}`;
  };

  return (
    <section
      className={`mega-menu-panel transition-all duration-500 ease-out ${
        isOpen ? "pointer-events-auto translate-y-0 opacity-100" : "pointer-events-none -translate-y-8 opacity-0"
      }`}
      aria-hidden={!isOpen}
      aria-label="Main menu"
    >
      <div className="mega-menu-grid">
        {menuGroups.map((group) => (
          <section key={group.title}>
            <h2 className="mega-menu-title">{group.title}</h2>
            <nav className="grid" aria-label={`${group.title} menu links`}>
              {group.links.map((link) => (
                <a
                  key={link}
                  className="mega-menu-link transition-opacity duration-300 hover:opacity-60"
                  href={getMenuHref(link)}
                  onClick={onClose}
                >
                  {link}
                </a>
              ))}
            </nav>
          </section>
        ))}
        <section>
          <h2 className="mega-menu-title">FEATURED</h2>
          <div className="featured-menu-grid">
            {products.map((product) => (
              <a
                key={product.id}
                className="featured-menu-item transition-opacity duration-300 hover:opacity-70"
                href="#featured"
                onClick={onClose}
              >
                <img className="featured-menu-image" src={product.image} alt={product.name} />
                <span>
                  <span className="featured-menu-name">
                    {product.name}
                  </span>
                  <span className="featured-menu-price">
                    {product.price}
                  </span>
                </span>
              </a>
            ))}
          </div>
        </section>
      </div>
    </section>
  );
}
