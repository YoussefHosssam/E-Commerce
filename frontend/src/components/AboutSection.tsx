export function AboutSection() {
  return (
    <section id="lookbook" className="border-t border-line bg-paper py-20 md:min-h-[85vh] md:py-[120px]">
      <div className="container-page grid items-start gap-10 md:grid-cols-[1fr_1.2fr] md:gap-[120px]">
        <h2 className="font-serif text-[clamp(64px,7vw,104px)] font-normal leading-[0.95] tracking-[-0.055em] text-ink">
          <span className="block">Lorem</span>
          <span className="block">ipsum</span>
        </h2>
        <div className="grid max-w-[560px] gap-6 text-[13px] leading-[1.9] text-[rgba(52,51,57,0.7)]">
          <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent sed velit sed mauris
            placerat volutpat. Integer tempor neque vitae sem tincidunt, id dignissim tortor
            interdum.
          </p>
          <p>
            Suspendisse at massa sit amet justo luctus facilisis. Curabitur finibus, magna sed
            pretium tristique, mi elit lacinia neque, sed elementum arcu erat ac sem.
          </p>
          <p>
            Donec gravida lectus non sapien faucibus, vitae commodo erat porta. Morbi sed orci vel
            lorem fermentum varius in a lectus.
          </p>
          <a
            className="tiny-button mt-4 inline-flex w-fit rounded-full border border-[rgba(52,51,57,0.35)] px-6 py-3 text-ink transition-colors duration-300 hover:bg-ink hover:text-white"
            href="#featured"
          >
            Shop Now
          </a>
        </div>
      </div>
    </section>
  );
}
