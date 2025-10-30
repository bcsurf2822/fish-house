// Mapping of fish names to their local image paths
// Images are stored in the public/fish-images directory
const BASE_URL = "/fish-images";

export const fishImageMap = {
  // Tuna varieties
  tuna: `${BASE_URL}/bluefin-tuna.jpg`,
  "bluefin tuna": `${BASE_URL}/bluefin-tuna.jpg`,
  "yellowfin tuna": `${BASE_URL}/yellowfin-tuna.jpg`,
  "albacore tuna": `${BASE_URL}/bluefin-tuna.jpg`,
  "skipjack tuna": `${BASE_URL}/bluefin-tuna.jpg`,
  "bigeye tuna": `${BASE_URL}/bluefin-tuna.jpg`,

  // Salmon varieties
  salmon: `${BASE_URL}/atlantic-salmon.jpg`,
  "atlantic salmon": `${BASE_URL}/atlantic-salmon.jpg`,
  "pacific salmon": `${BASE_URL}/atlantic-salmon.jpg`,
  "king salmon": `${BASE_URL}/atlantic-salmon.jpg`,
  "sockeye salmon": `${BASE_URL}/atlantic-salmon.jpg`,

  // Mackerel varieties
  mackerel: `${BASE_URL}/king-mackerel.jpg`,
  "king mackerel": `${BASE_URL}/king-mackerel.jpg`,
  "spanish mackerel": `${BASE_URL}/king-mackerel.jpg`,
  "atlantic mackerel": `${BASE_URL}/king-mackerel.jpg`,

  // Billfish
  swordfish: `${BASE_URL}/swordfish.jpg`,
  marlin: `${BASE_URL}/marlin.jpg`,
  sailfish: `${BASE_URL}/sailfish.jpg`,

  // Reef and coastal fish
  "mahi mahi": `${BASE_URL}/mahi-mahi.jpg`,
  halibut: `${BASE_URL}/pacific-halibut.jpg`,
  "pacific halibut": `${BASE_URL}/pacific-halibut.jpg`,
  grouper: `${BASE_URL}/groupers.jpg`,
  groupers: `${BASE_URL}/groupers.jpg`,
  snapper: `${BASE_URL}/snapper.jpg`,
  anchovy: `${BASE_URL}/anchovy.jpg`,
  anchovies: `${BASE_URL}/anchovy.jpg`,
  wrasse: `${BASE_URL}/humphead-wrasse.jpg`,
  "humphead wrasse": `${BASE_URL}/humphead-wrasse.jpg`,
  trevally: `${BASE_URL}/giant-trevally.jpg`,
  "giant trevally": `${BASE_URL}/giant-trevally.jpg`,
  tarpon: `${BASE_URL}/tarpon.jpg`,
  "flying fish": `${BASE_URL}/flying-fish.jpg`,
  pompano: `${BASE_URL}/pompano.jpg`,
  sturgeon: `${BASE_URL}/sturgeon.jpg`,
};

export const getFishImage = (fishName) => {
  const normalizedName = fishName.toLowerCase().trim();

  // Exact match
  if (fishImageMap[normalizedName]) {
    return fishImageMap[normalizedName];
  }

  // Partial match fallback
  for (const [key, value] of Object.entries(fishImageMap)) {
    if (normalizedName.includes(key) || key.includes(normalizedName)) {
      return value;
    }
  }

  // Default fallback image
  return `${BASE_URL}/bluefin-tuna.jpg`;
};
