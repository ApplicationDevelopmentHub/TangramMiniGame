# Tangram Mini Game

## 📦 Build

A desktop build is included inside the **DesktopExe/** folder.

Download and run the executable to play.

---

## 🎮 How to Play

- Click on a piece to select it.
- The selected piece moves to the left preview area.
- Use **Arrow Keys** to rotate the piece.
- Drag the piece onto the template to place it.

---

## 🧩 Snapping Logic

- Snapping is based on proximity to template slots.
- Pieces will snap when close enough and correctly rotated.
- If placement fails, the piece returns to the preview area.

---

## ⚠️ Known Limitations

- The **Square** and **Parallelogram** currently snap in only one rotation orientation.
- If snapping fails, try rotating through all angles.
- Each selected piece was scaled for interaction phase, but never was restored back to original scale after snapping, so the snapped pieces do not perfectly align.

---
