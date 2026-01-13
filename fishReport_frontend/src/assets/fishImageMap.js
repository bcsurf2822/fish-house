// Mapping of fish names to their local image paths
// Images are stored in the public/fish-images directory
const BASE_URL = "/fish-images";

export const fishImageMap = {
  // Tuna varieties
  tuna: `${BASE_URL}/bluefin-tuna.png`,
  "bluefin tuna": `${BASE_URL}/bluefin-tuna.png`,
  "yellowfin tuna": `${BASE_URL}/yellofin-tuna.png`,
  "albacore tuna": `${BASE_URL}/bluefin-tuna.png`,
  "skipjack tuna": `${BASE_URL}/bluefin-tuna.png`,
  "bigeye tuna": `${BASE_URL}/bluefin-tuna.png`,

  // Salmon varieties
  salmon: `${BASE_URL}/salmon.png`,
  "atlantic salmon": `${BASE_URL}/salmon.png`,
  "pacific salmon": `${BASE_URL}/salmon.png`,
  "king salmon": `${BASE_URL}/salmon.png`,
  "sockeye salmon": `${BASE_URL}/salmon.png`,

  // Mackerel varieties
  mackerel: `${BASE_URL}/king-mackerel.png`,
  "king mackerel": `${BASE_URL}/king-mackerel.png`,
  "spanish mackerel": `${BASE_URL}/king-mackerel.png`,
  "atlantic mackerel": `${BASE_URL}/king-mackerel.png`,

  // Billfish
  swordfish: `${BASE_URL}/swordfish.png`,
  marlin: `${BASE_URL}/blue-marlin.png`,
  "blue marlin": `${BASE_URL}/blue-marlin.png`,
  sailfish: `${BASE_URL}/sailfish.png`,

  // Reef and coastal fish
  "mahi mahi": `${BASE_URL}/mahi-mahi.png`,
  halibut: `${BASE_URL}/halibut.png`,
  "pacific halibut": `${BASE_URL}/halibut.png`,
  grouper: `${BASE_URL}/grouper.png`,
  groupers: `${BASE_URL}/grouper.png`,
  snapper: `${BASE_URL}/snapper.png`,
  anchovy: `${BASE_URL}/anchovy.png`,
  anchovies: `${BASE_URL}/anchovy.png`,
  wrasse: `${BASE_URL}/humphead-wrasse.png`,
  "humphead wrasse": `${BASE_URL}/humphead-wrasse.png`,
  trevally: `${BASE_URL}/giant-trevaly.png`,
  "giant trevally": `${BASE_URL}/giant-trevaly.png`,
  tarpon: `${BASE_URL}/bluefin-tuna.png`,
  "flying fish": `${BASE_URL}/flying-fish.png`,
  pompano: `${BASE_URL}/pompano.png`,
  sturgeon: `${BASE_URL}/sturgeon.png`,
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
  return `${BASE_URL}/bluefin-tuna.png`;
};
