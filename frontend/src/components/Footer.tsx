const footerColumns = [
  { title: "SOLVE", links: ["Home", "Rooms", "Journal", "Contact"] },
  { title: "SHOP", links: ["Home", "Rooms", "Lookbook", "FAQ"] },
  { title: "WEBFLOW", links: ["Styleguide", "Licensing", "Changelog"] },
  { title: "SOCIAL", links: ["Instagram", "Facebook", "Twitter"] },
];

export function Footer() {
  return (
    <footer id="site-footer" className="border-t border-[rgba(52,51,57,0.08)] bg-paper pb-[70px] pt-[90px]">
      <div className="container-page">
        <div className="grid gap-12 text-center sm:grid-cols-2 sm:text-left lg:grid-cols-4">
          {footerColumns.map((column) => (
            <section key={column.title}>
              <h2 className="font-serif text-[32px] font-normal leading-none tracking-[-0.055em] text-ink">{column.title}</h2>
              <nav className="mt-8 grid gap-4" aria-label={`${column.title} footer links`}>
                {column.links.map((link) => (
                  <a key={link} className="eyebrow-link text-muted" href="#top">
                    {link}
                  </a>
                ))}
              </nav>
            </section>
          ))}
        </div>
        <p className="mt-20 text-center text-[10px] uppercase leading-[1.7] tracking-[0.08em] text-muted">
          &copy; 2022 Made by Pawel Gola. Powered by Webflow.
        </p>
      </div>
    </footer>
  );
}
